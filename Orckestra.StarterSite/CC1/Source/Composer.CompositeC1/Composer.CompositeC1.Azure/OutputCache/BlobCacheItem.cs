using System;

namespace Orckestra.Composer.CompositeC1.Azure.OutputCache
{
    [Serializable]
    internal class BlobCacheItem
    {
        public DateTime UtcExpire { get; set; }

        public object Item { get; set; }
    }
}
