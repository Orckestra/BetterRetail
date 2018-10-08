using System;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Cache
{
    public abstract class CacheExclusion
    {
        public string[] UrlPatterns { get; set; }

        protected CacheExclusion(string[] urlPatterns)
        {
            UrlPatterns = urlPatterns;
        }

        /// <summary>
        /// Determines if the current Cache Exclusion rule should be applied to given Uri.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public virtual bool ShouldApplyExclusion(Uri requestUri)
        {
            var path = requestUri.PathAndQuery;
            var isMatch = UrlPatterns.Any(pattern => path.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase));

            return isMatch;
        }

        /// <summary>
        /// Determines if the page accessed with URI should be cached or not.
        /// </summary>
        /// <param name="requestUri">Uri to evaluate.</param>
        /// <returns>True if should be cached. False if should NOT be cached.</returns>
        public abstract bool ShouldCache(Uri requestUri);
    }
}
