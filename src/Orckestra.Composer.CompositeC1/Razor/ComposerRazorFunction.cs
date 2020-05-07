﻿using Composite.AspNet.Razor;
using Composite.Core;
using Composite.Core.Instrumentation;
using Composite.Core.Routing.Pages;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.ExperienceManagement.Configuration;
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
        public void RedirectNotAuthorized()
        {
            var context = HttpContext.Current;

            var websiteId = SitemapNavigator.CurrentHomePageId;
            var userData = (context.User.Identity as System.Web.Security.FormsIdentity)?.Ticket.UserData;

            bool authorized = context.User.Identity.IsAuthenticated && userData == websiteId.ToString();
            if (authorized) return;

            var siteConfiguration = ServiceLocator.GetService<ISiteConfiguration>();
            var pageService = ServiceLocator.GetService<IPageService>();
            var loginPageId = siteConfiguration.GetPagesConfiguration().LoginPageId;
            var culture = C1PageRoute.PageUrlData?.LocalizationScope;
            string loginUrl = pageService.GetPageUrl(loginPageId, culture);

            context.Response.Redirect(loginUrl);
        }

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
