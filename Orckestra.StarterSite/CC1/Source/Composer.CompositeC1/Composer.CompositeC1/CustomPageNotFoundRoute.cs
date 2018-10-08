using System.Web;
using System.Web.Routing;
using Composite.Core.WebClient;

namespace Orckestra.Composer.CompositeC1
{
    internal class CustomPageNotFoundRoute : Route
    {


        public CustomPageNotFoundRoute()
            : base("{*url}", new CustomPageNotFoundRouteHandler())
        {
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // Skipping root request
            if (httpContext.Request.RawUrl.Length == UrlUtils.PublicRootPath.Length + 1)
            {
                return null;
            }

            return base.GetRouteData(httpContext);
        }
    }
}