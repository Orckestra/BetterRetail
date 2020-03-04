using Composite.AspNet.Razor;
using Composite.Core.Instrumentation;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


// ReSharper disable once CheckNamespace
namespace Composer.Razor
{
    public abstract class ComposerRazorFunction : RazorFunction
    {
        public IHtmlString Partial(string viewName, object model)
        {
            return Partial(CreateController(Context), viewName, model);
        }

        public IHtmlString Partial(string controllerName, string viewName,
            object model)
        {
            return Partial(CreateController(Context, controllerName), viewName, model);
        }

        private static IHtmlString Partial(Controller controller, string viewName,
            object model)
        {
            var result = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);
            var viewData = new ViewDataDictionary()
            {
                Model = model
            };
            var sw = new StringWriter();
            var viewContext = new ViewContext(controller.ControllerContext, result.View, viewData, new TempDataDictionary(), sw);
            using (Profiler.Measure($"Evaluating view '{viewName}'"))
            {
                result.View.Render(viewContext, sw);
            }
            return new HtmlString(sw.ToString());
        }

        public IHtmlString HelpBubble(string category, string key)
        {
            var helpText = WebPagesHtmlHelperExtensions.Localize(category, key);
            if (!string.IsNullOrEmpty(helpText))
            {
                return new HtmlString(Partial("HelpBubbleOpen", null).ToHtmlString() + helpText + Partial("HelpBubbleClose", null).ToHtmlString());
            }

            return new HtmlString("");
        }

        public IHtmlString ParsleyMessage(string category, string key, string dataParsleyKey)
        {
            var message = WebPagesHtmlHelperExtensions.Localize(category, key);
            if (!string.IsNullOrEmpty(message))
            {
                message = String.Format("data-parsley-{0}=\"{1}\"", dataParsleyKey, message);
                return new HtmlString(HttpUtility.HtmlDecode(message));
            }

            return new HtmlString("");
        }

        internal class DefaultController : Controller { }

        /// <summary>
        /// Creates an instance of an MVC controller
        /// </summary>
        /// <returns>Controller</returns>
        internal static Controller CreateController(HttpContextBase httpContext, string controllerName = null)
        {
            var controller = new DefaultController();
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName ?? controller.GetType().Name);
            controller.ControllerContext = new ControllerContext(httpContext, routeData, controller);
            return controller;
        }
    }
}
