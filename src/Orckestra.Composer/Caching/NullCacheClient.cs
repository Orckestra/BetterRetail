using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// A caching client that will not cache. It's the null implementation of the <see cref="ICacheClient"/>.
    /// </summary>
    public class NullCacheClient : ICacheClient
    {
        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static readonly NullCacheClient Default = new NullCacheClient();

        /// <summary>
        ///  Acquire a lock for the given key
        /// </summary>
        /// <param name="key">Key to be locked</param>
        /// <param name="timeout">Timeout for acquiring the lock</param>
        /// <returns>A cache locker thats need to be disposed</returns>
        public IDisposable AcquireLock(string key, TimeSpan timeout)
        {
            return null;
        }

        /// <summary>
        ///  Acquire a lock for the given key
        /// </summary>
        /// <param name="key">Key to be locked</param>
        /// <param name="timeout">Timeout for acquiring the lock</param>
        /// <returns>A cache locker thats need to be disposed</returns>
        public Task<IDisposable> AcquireLockAsync(string key, TimeSpan timeout)
        {
            return Task.FromResult((IDisposable)null);
        }

        /// <summary>
        /// Get value from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>Any value from the cache or default(T)</returns>
        public T Get<T>(string key)
        {
            return default(T);
        }

        /// <summary>
        /// Retrieves  value from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">Key of the value.</param>
        /// <returns>An instance of <see cref="ConditionalResult{T}"/> where HasValue is True if the value is retrieved from the cache, otherwise false.</returns>
        public ConditionalResult<T> TryGet<T>(string key)
        {
            return new ConditionalResult<T>();
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
            return Task.FromResult(default(T));
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
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item that to remove.</param>
        /// <returns>True if the item was in the cache, otherwise false.</returns>
        public bool Remove(string key)
        {
            return false;
        }

        /// <summary>
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item that to remove.</param>
        /// <returns>True if the item was in the cache, otherwise false.</returns>
        public Task<bool> RemoveAsync(string key)
        {
            return Task.FromResult(false);
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
        public void Set<T>(string key, T o, TimeSpan duration, CacheItemPriority policy, string parentKey = null)
        {
        }

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <param name="duration"> Duration of the value in the cache. </param>
        /// <param name="policy"> Priority of the item in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        public void Set<T>(IEnumerable<DataToStoreInCache<T>> items, TimeSpan duration, CacheItemPriority policy)
        {
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
        public Task SetAsync<T>(string key, T o, TimeSpan duration, CacheItemPriority policy, string parentKey = null)
        {
            return Task.FromResult(false);
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
            return Task.FromResult(false);
        }

        /// <summary>
        /// Initialize the client with key value settings.
        /// </summary>
        /// <param name="customSettings">Dictionary containing the settings.</param>
        public void Initialize(IDictionary<string, string> customSettings)
        {

        }

        /// <summary>
        /// Clear the content of this cache.
        /// </summary>
        public void Clear()
        {
            //there is nothing to do.
        }

        public bool CanEnumerateKeys
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets a list of all the keys contained inside this cache.
        /// </summary>
        /// <remarks>May not be implemented in all implementations of cache client.</remarks>
        public IEnumerable<string> Keys
        {
            get { throw new NotSupportedException(); }
        }
    }
}
