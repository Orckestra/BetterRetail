using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Routing;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
    public interface ISearchUrlProvider
    {
        /// <summary>
        /// Register routes for Search
        /// </summary>
        void RegisterRoutes(RouteCollection routeCollection);

        /// <summary>
        /// Builds the search URL
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The base search URL is null or empty. Unable to build the search URL.</exception>
        string BuildSearchUrl(BuildSearchUrlParam param);

        /// <summary>
        /// Builds the selected facets.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        IEnumerable<SearchFilter> BuildSelectedFacets(NameValueCollection queryString);
    }
}