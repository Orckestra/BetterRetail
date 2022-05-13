using System;

namespace Orckestra.Composer.CompositeC1.Cache
{
    public sealed class CategoryPageExclusion: CacheExclusion
    {
        public CategoryPageExclusion(params string[] urlPatterns)
            :base(urlPatterns) { }

        /// <summary>
        /// Determines if the page accessed with URI should be cached or not.
        /// </summary>
        /// <param name="requestUri">Uri to evaluate.</param>
        /// <returns>True if should be cached. False if should NOT be cached.</returns>
        public override bool ShouldCache(Uri requestUri)
        {
            var shouldCache = IsQueryStringEmpty(requestUri.Query);
            return shouldCache;
        }

        private bool IsQueryStringEmpty(string query)
        {
            return string.IsNullOrWhiteSpace(query);
        }
    }
}