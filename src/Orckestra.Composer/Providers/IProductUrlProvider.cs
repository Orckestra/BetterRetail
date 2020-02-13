using System.Web.Routing;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    public interface IProductUrlProvider
    {
        void RegisterRoutes(RouteCollection routeCollection);
        string GetProductUrl(GetProductUrlParam parameters);
    }
}