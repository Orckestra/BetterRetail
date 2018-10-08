using System;

namespace Orckestra.Composer.CompositeC1.Cache
{
    public sealed class PathExclusion : CacheExclusion
    {
        public PathExclusion(params string[] urlPatterns)
            :base(urlPatterns)
        {
            
        }

        /// <summary>
        /// Determines if the URI should not be cached.
        /// </summary>
        /// <param name="requestUri">Uri to evaluate.</param>
        /// <returns>True if should be excluded from cache. False if should be cached.</returns>
        public override bool ShouldCache(Uri requestUri)
        {
            return false;
        }
    }
}