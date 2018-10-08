using System.Web;
using System.Web.Routing;
using Composite.Core;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public static class UrlProviderHelper
    {
        public static string BuildUrlWithParams(string url, string returnUrl, RouteData routeData = null)
        {
            var urlBuilder = new UrlBuilder(url);

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                urlBuilder["ReturnUrl"] = HttpUtility.UrlEncode(returnUrl);
            }

            if (routeData != null)
            {
                foreach (var data in routeData.Values)
                {
                    urlBuilder[data.Key] = HttpUtility.UrlEncode(data.Value.ToString());
                }
            }

            return urlBuilder.ToString();
        }
    }
}
