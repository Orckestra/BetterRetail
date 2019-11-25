using System;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Cache
{
    public class PageCacheEvaluator
    {
        public bool ShouldCache(Uri requestUri)
        {
            var applicableExclusion =
                CacheConfiguration.CacheExclusions.FirstOrDefault(ce => ce.ShouldApplyExclusion(requestUri));

            if (applicableExclusion != null)
            {
                var shouldCache = applicableExclusion.ShouldCache(requestUri);
                return shouldCache;
            }

            return true;
        }
    }
}
