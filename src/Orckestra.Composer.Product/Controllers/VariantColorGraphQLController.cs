using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Composite.Core;
using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.NewtonsoftJson;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using Orckestra.Composer.Product.GraphQL.Interfaces;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Product.Controllers
{
    [ValidateLanguage]
    [RoutePrefix("composite/api/variantcolorgraphql")]
    public class VariantColorGraphQLController : ApiController
    {
        private readonly IVariantColorSchema _schema;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;
        private readonly IServiceProvider _serviceProvider;

        public VariantColorGraphQLController(
            IDocumentExecuter executer,
            IDocumentWriter writer,
            IVariantColorSchema schema,
            IServiceProvider serviceProvider)
        {
            _executer = executer;
            _writer = writer;
            _schema = schema;
            _serviceProvider = serviceProvider;
        }

        public VariantColorGraphQLController()
        {
            _executer = ServiceLocator.GetService<IDocumentExecuter>();
            _writer = ServiceLocator.GetService<IDocumentWriter>();
            _schema = ServiceLocator.GetService<IVariantColorSchema>();
            _serviceProvider = ServiceLocator.GetService<IServiceProvider>();
        }

        [HttpGet]
        [Route("status")]
        public virtual IHttpActionResult Status()
        {
            return Ok(nameof(Ok));
        }

        [HttpPost]
        [Route("query")]
        public async Task<HttpResponseMessage> PostAsync(HttpRequestMessage request, GraphQLQuery query)
        {
            var inputs = query.Variables.ToInputs();
            var queryToExecute = query.Query;

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = _schema;
                _.Query = queryToExecute;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
                _.RequestServices = _serviceProvider;

                _.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };

            }).ConfigureAwait(false);

            var httpResult = result.Errors?.Count > 0
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.OK;

            var json = await _writer.WriteToStringAsync(result);

            var response = request.CreateResponse(httpResult);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return response;
        }
    }

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public Newtonsoft.Json.Linq.JObject Variables { get; set; }
    }

}
