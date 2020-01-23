using Composite.Core.Instrumentation;
using Composite.Core.Localization;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;
using Composite.Functions;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace Orckestra.Composer.CompositeC1.Api
{
    //[ValidateLanguage]
    //[JQueryOnlyFilter]
    public class PartialController : ApiController
    {
        protected string ContentType = "text/html";

        public HttpContextBase HttpContext { get; }
        public ILazyPartialProvider LazyPartialProvider { get; }

        public PartialController(HttpContextBase httpContext, ILazyPartialProvider lazyPartialProvider)
        {
            HttpContext = httpContext;
            LazyPartialProvider = lazyPartialProvider;
        }
        [HttpGet]
        [ActionName("test2")]
        public virtual IHttpActionResult Test2()

        {
            return Ok("test2");
        }


        [HttpPost]
        [ActionName("render")]
        public virtual IHttpActionResult/*HttpResponseMessage*/ Render(/*HttpRequestMessage request*/[FromBody] string body)
        {
            //var body = request.Content.ReadAsStringAsync().Result;
            var decrypted = LazyPartialProvider.UnprotectFuntionCall(body);
            if(decrypted != null) { 
            var cultureInfo = new CultureInfo("en-CA");

            HttpContext.RewritePath(HttpContext.Request.FilePath, HttpContext.Request.PathInfo, decrypted.QueryString);

                using (var data = new DataConnection(PublicationScope.Published, cultureInfo))
                {
                    // Grab a function object to execute
                    IFunction function = FunctionFacade.GetFunction(decrypted.FunctionName);

                    var productPage = data.Get<IPage>().First();
                    PageRenderer.CurrentPage = productPage;

                    // Execute the function, passing all query string parameters as input parameters
                    var functionResult = (XhtmlDocument)FunctionFacade.Execute<object>(function, decrypted.Parameters.ToDictionary(d => d.Key, d => (object)d.Value));


                    // output result
                    if (functionResult != null)
                    {
                        var functionContext = new FunctionContextContainer();

                        PageRenderer.ExecuteEmbeddedFunctions(functionResult.Root, functionContext);

                        //PageRenderer.ProcessXhtmlDocument(functionResult, productPage);

                        using (Profiler.Measure("Normalizing XHTML document"))
                        {
                            PageRenderer.NormalizeXhtmlDocument(functionResult);
                        }

                        using (Profiler.Measure("Resolving relative paths"))
                        {
                            PageRenderer.ResolveRelativePaths(functionResult);
                        }


                        using (Profiler.Measure("Parsing localization strings"))
                        {
                            LocalizationParser.Parse(functionResult);
                        }

                        using (Profiler.Measure("Converting URLs from internal to public format (XhtmlDocument)"))
                        {
                            InternalUrls.ConvertInternalUrlsToPublic(functionResult);
                        }

                        //TODO: Update C1 Version
                        //PageRenderer.ProcessDocumentHead(functionResult);

                        //var contentType = ContentType;
                        //if (functionResult is XNode && function.ReturnType != typeof(Composite.Core.Xml.XhtmlDocument))
                        //    contentType = "text/xml";


                        StringBuilder sb = new StringBuilder();

                        foreach (var node in functionResult.Body.Nodes())
                        {
                            sb.Append(node.ToString());
                        }

                        return Json(sb.ToString());
                        //var response = new HttpResponseMessage()
                        //{
                        //    Content = new StringContent(sb.ToString()),

                        //};
                        //response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                        //return response;
                    }
                }
               
            }

            return NotFound();
            //return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }
    }

    public class LazyFunctionCall
    {
        public string FunctionName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string QueryString { get; set; }

        public LazyFunctionCall()
        {
        }
        public LazyFunctionCall(string functionName, Dictionary<string, string> parameters, string queryString)
        {
            FunctionName = functionName;
            Parameters = parameters;
            QueryString = queryString;
        }

        public override bool Equals(object obj)
        {
            return obj is LazyFunctionCall other &&
                   FunctionName == other.FunctionName &&
                   EqualityComparer<Dictionary<string, string>>.Default.Equals(Parameters, other.Parameters) &&
                   QueryString == other.QueryString;
        }

        public override int GetHashCode()
        {
            var hashCode = -1561600887;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FunctionName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, string>>.Default.GetHashCode(Parameters);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(QueryString);
            return hashCode;
        }
    }
}
