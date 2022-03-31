using System;
using System.Web;
using System.Web.Routing;
using Composite.Core.Routing;
using Composite.Core.WebClient;
using Orckestra.Composer.Providers;
using ServiceStack;

namespace Orckestra.Composer.CompositeC1
{
    public abstract class C1HttpApplication : HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            ApplicationLevelEventHandlers.LogRequestDetails = false;
            ApplicationLevelEventHandlers.LogApplicationLevelErrors = false;

            ApplicationLevelEventHandlers.Application_Start(sender, e);

            RegisterRoutes(RouteTable.Routes);

            PclExport.Instance = new ComposerPclExport();
        }

        protected virtual void RegisterRoutes(RouteCollection routes)
        {
            Routes.RegisterPageRoute(routes);

            // If necessary, add the standard MVC route "{controller}/{action}/{id}" after registering the C1 page route

            routes.Add("Http404 route", new CustomPageNotFoundRoute());
        }

        /// <summary>
        /// This will Register the NoCache provider if it's not present in the list. HOWEVER, if no provider is
        /// registered at all, it won't be able to work.
        /// </summary>

        protected virtual void Application_End(object sender, EventArgs e)
        {
            ApplicationLevelEventHandlers.Application_End(sender, e);
        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            ApplicationLevelEventHandlers.Application_BeginRequest(sender, e);
        }

        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
            ApplicationLevelEventHandlers.Application_EndRequest(sender, e);
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            if (HandleHttp404Exception(sender as HttpApplication))
            {
                return;
            }
            ApplicationLevelEventHandlers.Application_Error(sender, e);
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            return ApplicationLevelEventHandlers.GetVaryByCustomString(context, custom) ?? base.GetVaryByCustomString(context, custom);
        }


        private bool HandleHttp404Exception(HttpApplication httpApplication)
        {
            var exception = httpApplication.Server.GetLastError();
            var is404 = IsHttp404(exception);

            if (is404)
            {
                var httpContext = httpApplication.Context;
                var pageNotFoundUrlProvider = ComposerHost.Current.Resolve<IPageNotFoundUrlProvider>();
                httpContext.Response.StatusCode = 404;
                Server.ClearError();
                var url = httpContext.Request.Url.ToString();
                var pageNotFoundUrl = pageNotFoundUrlProvider.Get404PageUrl(url);
                httpContext.Response.Redirect(pageNotFoundUrl);
                return true;
            }

            return false;
        }

        private bool IsHttp404(Exception ex)
        {
            if (ex == null) return false;
            return (ex is HttpException httpException && httpException.GetHttpCode() == 404) || IsHttp404(ex.InnerException);
        }
    }
}
