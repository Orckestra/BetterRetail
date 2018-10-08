using System.Collections.Generic;

namespace Orckestra.Composer.CompositeC1.Cache
{
    public static class CacheConfiguration
    {
        public static bool NotifyCacheInHeader { get; set; }
        public static List<CacheExclusion> CacheExclusions { get; set; }

        static CacheConfiguration()
        {
            NotifyCacheInHeader = false;
            CacheExclusions = new List<CacheExclusion>();
        }
    }
}