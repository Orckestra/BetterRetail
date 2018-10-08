using System.Web.Mvc;
using System.Web.Routing;

namespace Orckestra.Composer.CompositeC1.Mvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}
