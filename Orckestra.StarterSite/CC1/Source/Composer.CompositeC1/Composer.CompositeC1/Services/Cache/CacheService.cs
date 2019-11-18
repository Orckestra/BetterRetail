using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    internal class CacheService : ICacheService, IDisposable
    {
        private readonly ConcurrentDictionary<(string, Type, Type), object> _cacheStore = new ConcurrentDictionary<(string, Type, Type), object>();
        public ICacheStore<K, V> GetStore<K, V>(string name, int maximumSize) where V : class
        {
            return GetOrCreateStore<K, V>(name, maximumSize);
        }

        public ICacheStore<K, V> GetStore<K, V>(string name) where V : class
        {
            return GetOrCreateStore<K, V>(name);
        }

        public ICacheStore<K, V> GetStoreWithDependencies<K, V>(string name, int maximumSize, params CacheDependentEntry[] dependentEntities) where V : class
        {
            return GetOrCreateStore<K, V>(name, dependentEntities, maximumSize);
        }

        public ICacheStore<K, V> GetStoreWithDependencies<K, V>(string name, params CacheDependentEntry[] dependentEntities) where V : class
        {
            return GetOrCreateStore<K, V>(name, dependentEntities);
        }

        private ICacheStore<K, V> GetOrCreateStore<K, V>(string name, int? maximumSize = null) where V : class
        {
            return (ICacheStore<K, V>)_cacheStore.GetOrAdd((name, typeof(K), typeof(V)), _ => CreateStore<K, V>(name, maximumSize));
        }

        private CacheStore<K, V> CreateStore<K, V>(string name, int? maximumSize = null) where V : class
        {
            if (maximumSize.HasValue)
            {
                return new CacheStore<K, V>(name, maximumSize.Value);
            }
            else
            {
                return new CacheStore<K, V>(name);
            }
        }

        private ICacheStore<K, V> GetOrCreateStore<K, V>(string name, CacheDependentEntry[] dependentEntities, int? maximumSize = null) where V : class
        {
            return (ICacheStore<K, V>)_cacheStore.GetOrAdd((name, typeof(K), typeof(V)), _ => CreateStore<K, V>(name, dependentEntities, maximumSize));
        }

        private DependentCacheStore<K, V> CreateStore<K, V>(string name, CacheDependentEntry[] dependentEntities, int? maximumSize = null) where V : class
        {
            if (maximumSize.HasValue)
            {
                return new DependentCacheStore<K, V>(name, maximumSize.Value, dependentEntities);
            }
            else
            {
                return new DependentCacheStore<K, V>(name, dependentEntities);
            }
        }


        public void Dispose()
        {
            var stores = _cacheStore.Values.OfType<IDisposable>().ToList();
            _cacheStore.Clear();

            foreach (var store in stores)
            {
                store.Dispose();
            }
        }
    };
}