using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Composite.Core.Configuration;
using Composite.Core.Extensions;
using Composite.Core.Routing.Pages;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.CompositeC1
{
    public class RequestInterceptorHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += context_PreRequestHandlerExecute;
        }

        private void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (!SystemSetupFacade.IsSystemFirstTimeInitialized)
            {
                return;
            }

            HttpContext httpContext = (sender as HttpApplication).Context;
            Page page = httpContext.Handler as Page;

            if (page != null && !string.IsNullOrWhiteSpace(C1PageRoute.GetPathInfo()))
            {
                page.PreInit += (a, b) => CheckThatPathInfoHasBeenUsed(httpContext, page);
            }
        }

        private static void CheckThatPathInfoHasBeenUsed(HttpContext httpContext, Page page)
        {
            if (C1PageRoute.PathInfoUsed)
            {
                return;
            }

            if (!ServeCustomPageNotFoundPage(httpContext))
            {
                page.Response.StatusCode = 404;
            }

            page.Response.End();
        }

        private static bool ServeCustomPageNotFoundPage(HttpContext httpContext)
        {
            string rawUrl = httpContext.Request.RawUrl;
            var customPageNotFoundProvider = ComposerHost.Current.Resolve<IPageNotFoundUrlProvider>();
            var customPageNotFoundUrl = customPageNotFoundProvider.Get404PageUrl(rawUrl);

            if (string.IsNullOrEmpty(customPageNotFoundUrl))
            {
                return false;
            }
            if (rawUrl == customPageNotFoundUrl || httpContext.Request.Url.PathAndQuery == customPageNotFoundUrl)
            {
                throw new HttpException(404, "'Page not found' wasn't handled. Url: '{0}'".FormatWith(rawUrl));
            }

            if (HttpRuntime.UsingIntegratedPipeline && customPageNotFoundUrl.StartsWith("/"))
            {
                httpContext.Server.TransferRequest(customPageNotFoundUrl);
                return true;
            }
            httpContext.Response.Redirect(customPageNotFoundUrl, true);
            throw new InvalidOperationException("This code should not be reachable");
        }

        public void Dispose()
        {
        }
    }
}
