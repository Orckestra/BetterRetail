using System;
using System.Web;
using System.Web.Routing;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.CompositeC1
{

    internal class CustomPageNotFoundRouteHandler : IRouteHandler
    {
        protected IPageNotFoundUrlProvider PageNotFoundUrlProvider { get; private set; }

        public CustomPageNotFoundRouteHandler()
        {

             PageNotFoundUrlProvider = ComposerHost.Current.Resolve<IPageNotFoundUrlProvider>();
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var httpContext = HttpContext.Current;

            var customPageNotFoundUrl = PageNotFoundUrlProvider.Get404PageUrl(httpContext.Request.RawUrl);

            if (!string.IsNullOrEmpty(customPageNotFoundUrl))
            {
                httpContext.Response.Redirect(customPageNotFoundUrl, true);
            }

            return EmptyHttpHandler.Instance;
        }
    }
}