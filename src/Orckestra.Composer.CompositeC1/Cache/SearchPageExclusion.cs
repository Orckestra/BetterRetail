using System;
using System.Collections.Specialized;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Cache
{
    public sealed class SearchPageExclusion: CacheExclusion
    {
        public SearchPageExclusion(params string[] urlPatterns)
            :base(urlPatterns)
        {
            
        }

        /// <summary>
        /// Determines if the page accessed with URI should be cached or not.
        /// </summary>
        /// <param name="requestUri">Uri to evaluate.</param>
        /// <returns>True if should be cached. False if should NOT be cached.</returns>
        public override bool ShouldCache(Uri requestUri)
        {
            var queryString = GetRequestQueryString(requestUri);
            var shouldCache = IsQueryStringOnlyComposedOfKeywords(queryString);

            return shouldCache;
        }

        private NameValueCollection GetRequestQueryString(Uri requestUri)
        {
            var queryString = requestUri.Query;
            var queries = HttpUtility.ParseQueryString(queryString);

            return queries;
        }

        private bool IsQueryStringOnlyComposedOfKeywords(NameValueCollection queries)
        {
            var isOnlyKeywords = queries.Count == 1 &&
                                 string.Equals(queries.GetKey(0), "keywords", StringComparison.CurrentCultureIgnoreCase);

            return isOnlyKeywords;
        }
    }
}
