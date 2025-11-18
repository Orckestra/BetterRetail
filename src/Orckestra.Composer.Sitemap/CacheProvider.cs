using System;
using System.Runtime.Caching;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public static class CacheProvider
    {
        private static readonly ObjectCache _cache = MemoryCache.Default;

        public static void Set(string key, object value, int minutes = 30)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) return;

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(minutes)
            };

            _cache.Set(key, value, policy);
        }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) return default;
            var item = _cache.Get(key);

            return item is T typedItem ? typedItem : default;
        }

        public static void Clear()
        {
            foreach (var item in _cache)
            {
                _cache.Remove(item.Key);
            }
        }
    }
}
