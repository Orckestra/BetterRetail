using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;
using Composite.AspNet.MvcFunctions;
using Composite.Core;
using Composite.Core.Application;
using Composite.Core.ResourceSystem;
using Composite.Functions;
using Orckestra.Composer.C1CMS.Queries.Controllers;

namespace Orckestra.Composer.C1CMS.Queries
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {

        public static void OnBeforeInitialize()
        {

        }

        public static void OnInitialized()
        {
            if (!HostingEnvironment.IsHosted) return;

            Log.LogInformation("SearchQueryBuilder", "OnInitialized");

            var functions = MvcFunctionRegistry.NewFunctionCollection();
            RegisterFunctions(functions);
            RegisterFunctionRoutes(functions);

        }

        private static void RegisterFunctions(FunctionCollection functions)
        {
            RegisterSearchQueryController<SearchQueryController>(functions, "Merchandising");
            RegisterSearchQueryController<ProductSetController>(functions, "ProductSet");

            functions.AutoDiscoverFunctions(typeof(StartupHandler).Assembly);
        }

        private static void RegisterFunctionRoutes(FunctionCollection functions)
        {
            functions.RouteCollection.MapMvcAttributeRoutes();
            functions.RouteCollection.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );
        }

        private static void RegisterSearchQueryController<T>(FunctionCollection functions, string searchQueryType) where T : Controller
        {
            WidgetFunctionProvider searhQueryListWidgetFunctionProvider = new WidgetFunctionProvider(XElement.Parse($@"<f:widgetfunction xmlns:f='http://www.composite.net/ns/function/1.0' name='Composite.Widgets.String.Selector'>
                <f:param name='Options'>
                        <f:function name='Composer.SearchQuery.Functions.GetSearchQueryList'>
                             <f:param name='type' value='{searchQueryType}' />
                        </f:function>
                </f:param>
            </f:widgetfunction>"));
        }

    }
}