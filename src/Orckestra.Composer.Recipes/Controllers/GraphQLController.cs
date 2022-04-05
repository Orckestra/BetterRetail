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
using Orckestra.Composer.Recipes.GraphQL.Interfaces;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Recipes.Controllers
{
    [ValidateLanguage]
    [RoutePrefix("composite/api/recipe/graphql")]
    public class GraphQLController : ApiController
    {
        private readonly IRecipeSchema _schema;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;

        public GraphQLController(
            IDocumentExecuter executer,
            IDocumentWriter writer,
            IRecipeSchema schema)
        {
            _executer = executer;
            _writer = writer;
            _schema = schema;
        }

        public GraphQLController()
        {
            _executer = ServiceLocator.GetService<IDocumentExecuter>();
            _writer = ServiceLocator.GetService<IDocumentWriter>();
            _schema = ServiceLocator.GetService<IRecipeSchema>();
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
