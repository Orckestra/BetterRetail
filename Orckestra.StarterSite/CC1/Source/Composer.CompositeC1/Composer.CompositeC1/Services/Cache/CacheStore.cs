using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Composite.Data.Caching;

namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    internal class CacheStore<K, V> : ICacheStore<K, V>, IDisposable where V : class
    {
        private readonly Cache<K, CachedValue<V>> _cache;

        public CacheStore(string name, int maximumSize)
        {
            _cache = new Cache<K, CachedValue<V>>(name, maximumSize);
        }

        public CacheStore(string name)
        {
            _cache = new Cache<K, CachedValue<V>>(name);
        }

        public string Name => _cache.Name;

        public void Add(K key, V value) => _cache.Add(key, new CachedValue<V>(value));

        public V Get(K key) => _cache.Get(key)?.Value;

        public IEnumerable<K> GetKeys() => _cache.GetKeys();

        public void Remove(K key) => _cache.Remove(key);

        public void Clear() => _cache.Clear();

        private readonly ConcurrentDictionary<K, object> _locks = new ConcurrentDictionary<K, object>();
        public V GetOrAdd(K key, Func<K, V> loadMethod)
        {
            var cacheObject = _cache.Get(key);
            if (cacheObject != null)
                return cacheObject.Value;

            var syncObj = _locks.GetOrAdd(key, _ => new object());
            lock (syncObj)
            {
                try
                {
                    cacheObject = _cache.Get(key);
                    if (cacheObject != null)
                        return cacheObject.Value;

                    var value = loadMethod(key);
                    Add(key, value);
                    return value;
                }
                finally
                {
                    _locks.TryRemove(key, out _);
                }
            }
        }


        private class CachedValue<T>
        {
            public CachedValue(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        public virtual void Dispose()
        {
            _cache.Clear();
        }
    };

}