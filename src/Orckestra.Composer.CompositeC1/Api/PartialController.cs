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
using System.Web.UI;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Api
{
    /// <summary>
    /// Render C1 Functions
    /// </summary>
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class FunctionController : ApiController
    {
        protected string ContentType = "text/html";
        private const string CacheProfileName = "C1Page";

        public IComposerContext ComposerContext { get; }
        public HttpContextBase HttpContext { get; }
        public ILazyFunctionCallDataProvider LazyFunctionCallDataProvider { get; }

        public FunctionController(IComposerContext composerContext, HttpContextBase httpContext, ILazyFunctionCallDataProvider lazyFunctionCallDataProvider)
        {
            ComposerContext = composerContext;
            HttpContext = httpContext;
            LazyFunctionCallDataProvider = lazyFunctionCallDataProvider;
        }


        [HttpPost]
        [ActionName("body")]
        public virtual IHttpActionResult Body([FromBody] string body)
        {
            InitializeFullPageCaching(System.Web.HttpContext.Current);

            if (string.IsNullOrWhiteSpace(body)) NotFound();

            var decrypted = LazyFunctionCallDataProvider.UnprotectFunctionCall(body);
            
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


        public static void InitializeFullPageCaching(HttpContext context)
        {
            using (var page = new CacheableEmptyPage())
            {
                page.ProcessRequest(context);
            }
        }


        private class CacheableEmptyPage : Page
        {
            protected override void FrameworkInitialize()
            {
                base.FrameworkInitialize();

                // That's an equivalent of having <%@ OutputCache CacheProfile="C1Page" %> 
                // on an *.aspx page

                InitOutputCache(new OutputCacheParameters
                {
                    CacheProfile = CacheProfileName
                });
            }
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
