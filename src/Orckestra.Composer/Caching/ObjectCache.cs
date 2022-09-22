using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// The implemation of cache client using <see cref="ObjectCache"/>.
    /// </summary>
    public abstract class ObjectCacheClient : ICacheClient
    {
        private ObjectCache _objectCache;
        private readonly MemoryCacheLocker _locker = new MemoryCacheLocker();

        protected abstract ObjectCache CreateCache();

        /// <summary>
        /// Gets the current object cache.
        /// </summary>
        protected ObjectCache ObjectCache
        {
            get { return _objectCache = _objectCache ?? CreateCache(); }
        }

        /// <summary>
        ///  Acquire a lock for the given key
        /// </summary>
        /// <param name="key">Key to be locked</param>
        /// <param name="timeout">Timeout for acquiring the lock</param>
        /// <returns>A cache locker thats need to be disposed</returns>
        [Obsolete]
        public IDisposable AcquireLock(string key, TimeSpan timeout)
        {
            return _locker.Acquire(key, timeout);
        }

        /// <summary>
        ///  Acquire a lock for the given key
        /// </summary>
        /// <param name="key">Key to be locked</param>
        /// <param name="timeout">Timeout for acquiring the lock</param>
        /// <returns>A cache locker thats need to be disposed</returns>
        [Obsolete]
        public Task<IDisposable> AcquireLockAsync(string key, TimeSpan timeout)
        {
            return _locker.AcquireAsync(key, timeout);
        }

        /// <summary>
        /// Get value from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>Any value from the cache or default(T)</returns>
        public T Get<T>(string key)
        {
            return CacheClientHelper.Get<T>(this, key);
        }

        /// <summary>
        /// Retrieves  value from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">Key of the value.</param>
        /// <returns>An instance of <see cref="ConditionalResult{T}"/> where HasValue is True if the value is retrieved from the cache, otherwise false.</returns>
        public ConditionalResult<T> TryGet<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");

            var rawValue = ObjectCache.GetCacheItem(key);

            if (rawValue == null)
            {
                return new ConditionalResult<T>();
            }

            return new ConditionalResult<T>((T)rawValue.Value);
        }

        /// <summary>
        /// Get values from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="keys">A list of cache keys</param>
        /// <returns>A Dictionary{key, T} of values from the cache. If the key does not exists in the cache it will not be present in the Dictionary.</returns>
        public Dictionary<string, T> Get<T>(IEnumerable<string> keys)
        {
            return CacheClientHelper.GetSequential<T>(this, keys);
        }

        /// <summary>
        /// Get value from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>Any value from the cache or default(T)</returns>
        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        /// <summary>
        /// Retrieves  value from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">Key of the value.</param>
        /// <returns>An instance of <see cref="ConditionalResult{T}"/> where HasValue is True if the value is retrieved from the cache, otherwise false.</returns>
        public Task<ConditionalResult<T>> TryGetAsync<T>(string key)
        {
            return Task.FromResult(TryGet<T>(key));
        }

        /// <summary>
        /// Get values from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="keys">A list of cache keys</param>
        /// <returns>A Dictionary{key, T} of values from the cache. If the key does not exists in the cache it will not be present in the Dictionary.</returns>
        public Task<Dictionary<string, T>> GetAsync<T>(IEnumerable<string> keys)
        {
            return Task.FromResult(Get<T>(keys));
        }

        /// <summary>
        /// Adds or replace a value in the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">Key of the value.</param>
        /// <param name="o">Value being push in cache.</param>
        /// <param name="duration">Duration of the value in the cache.</param>
        /// <param name="policy">Priority of the item in the cache.</param>
        /// <param name="parentKey"> The key of the item that this item is a child of.</param>
        /// <exception cref="System.ArgumentException">key</exception>
        public void Set<T>(string key, T o, TimeSpan duration, CacheItemPriority policy, string parentKey = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");

            var cachePolicy = CreateCachePolicy(duration, policy);

            if (o == null)
            {
                Remove(key);
            }
            else
            {
                ObjectCache.Set(key, o, cachePolicy);
            }
        }

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <param name="duration"> Duration of the value in the cache. </param>
        /// <param name="policy"> Priority of the item in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        public void Set<T>(IEnumerable<DataToStoreInCache<T>> items, TimeSpan duration, CacheItemPriority policy)
        {
            CacheClientHelper.Set(this, items, duration, policy);
        }

        /// <summary>
        /// Adds or replace a value in the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">Key of the value.</param>
        /// <param name="o">Value being push in cache.</param>
        /// <param name="duration">Duration of the value in the cache.</param>
        /// <param name="policy">Priority of the item in the cache.</param>
        /// <param name="parentKey"> The key of the item that this item is a child of.</param>
        /// <exception cref="System.ArgumentException">key</exception>
        public Task SetAsync<T>(string key, T o, TimeSpan duration, CacheItemPriority policy, string parentKey = null)
        {
            Set(key, o, duration, policy, parentKey);
            return Task.FromResult(true);
        }

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <param name="duration"> Duration of the value in the cache. </param>
        /// <param name="policy"> Priority of the item in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        /// <returns>A async task</returns>
        public Task SetAsync<T>(IEnumerable<DataToStoreInCache<T>> items, TimeSpan duration, CacheItemPriority policy)
        {
            Set(items, duration, policy);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item that needs to be remove.</param>
        /// <returns>
        /// True if the item were in the cache, otherwise false.
        /// </returns>
        public bool Remove(string key)
        {
            var value = ObjectCache.Remove(key);
            return value != null;
        }

        /// <summary>
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item that needs to be remove.</param>
        /// <returns>
        /// True if the item were in the cache, otherwise false.
        /// </returns>
        public Task<bool> RemoveAsync(string key)
        {
            return Task.FromResult(Remove(key));
        }

        private System.Runtime.Caching.CacheItemPriority Convert(CacheItemPriority priority)
        {
            switch (priority)
            {
                case CacheItemPriority.NotRemovable:
                    return System.Runtime.Caching.CacheItemPriority.NotRemovable;
                default:
                    return System.Runtime.Caching.CacheItemPriority.Default;
            }
        }

        private CacheItemPolicy CreateCachePolicy(TimeSpan offset, CacheItemPriority priority)
        {
            return new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(offset),
                Priority = Convert(priority)
            };
        }

        /// <summary>
        /// Initialize the client with key value settings.
        /// </summary>
        /// <param name="customSettings">
        /// Dictionary containing the settings.
        /// </param>
        public virtual void Initialize(IDictionary<string, string> customSettings)
        {

        }

        /// <summary>
        /// Clear the content of this cache.
        /// </summary>
        public virtual void Clear()
        {
            foreach (var key in Keys)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the cache client can enumerate keys.
        /// </summary>
        public bool CanEnumerateKeys
        {
            get { return true; }
        }

        /// <summary>
        ///     Gets a list of all the keys contained inside this cache.
        /// </summary>
        /// <remarks>May not be implemented in all implementations of cache client.</remarks>
        public IEnumerable<string> Keys
        {
            get { return ObjectCache.Select(kvp => kvp.Key); }
        }
    }
}
