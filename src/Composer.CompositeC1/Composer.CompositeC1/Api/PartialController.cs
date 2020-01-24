using Composite.Core.Instrumentation;
using Composite.Core.Localization;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Functions;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class PartialController : ApiController
    {
        protected string ContentType = "text/html";

        public IComposerContext ComposerContext { get; }
        public HttpContextBase HttpContext { get; }
        public ILazyPartialProvider LazyPartialProvider { get; }

        public PartialController(IComposerContext composerContext, HttpContextBase httpContext, ILazyPartialProvider lazyPartialProvider)
        {
            ComposerContext = composerContext;
            HttpContext = httpContext;
            LazyPartialProvider = lazyPartialProvider;
        }


        [HttpPost]
        [ActionName("body")]
        public virtual IHttpActionResult Body([FromBody] string body)
        {
            if (string.IsNullOrWhiteSpace(body)) NotFound();

            var decrypted = LazyPartialProvider.UnprotectFunctionCall(body);
            
            if (decrypted == null) return NotFound();


            HttpContext.RewritePath(HttpContext.Request.FilePath, HttpContext.Request.PathInfo, decrypted.QueryString);

            using (var data = new DataConnection(PublicationScope.Published, ComposerContext.CultureInfo))
            {
                // Grab a function object to execute
                IFunction function = FunctionFacade.GetFunction(decrypted.FunctionName);

                PageRenderer.CurrentPage = PageManager.GetPageById(decrypted.PageId); ;

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

                    StringBuilder sb = new StringBuilder();

                    foreach (var node in functionResult.Body.Nodes())
                    {
                        sb.Append(node.ToString());
                    }

                    return Json(sb.ToString());
                }
            }

            return NotFound();
        }
    }

    public class LazyFunctionCall
    {
        public string FunctionName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string QueryString { get; set; }
        public Guid PageId { get; set; }
    }
}
