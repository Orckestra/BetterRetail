using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    ///     This interface defines the contract that must be implemented by all caching clients.
    /// </summary>
    public interface ICacheClient
    {
        /// <summary>
        /// Gets a value indicating whether the cache client can enumerate keys.
        /// </summary>
        bool CanEnumerateKeys { get; }

        /// <summary>
        ///     Gets a list of all the keys contained inside this cache.
        /// </summary>
        /// <remarks>May not be implemented in all implementations of cache client.</remarks>
        IEnumerable<string> Keys { get; }

        /// <summary>
        ///  Acquire a lock for the given key
        /// </summary>
        /// <param name="key">Key to be locked</param>
        /// <returns>A cache locker thats need to be disposed</returns>
        /// <param name="timeout">Timeout for acquiring the lock</param>
        IDisposable AcquireLock(string key, TimeSpan timeout);

        /// <summary>
        ///  Acquire a lock for the given key
        /// </summary>
        /// <param name="key">Key to be locked</param>
        /// <param name="timeout">Timeout for acquiring the lock</param>
        /// <returns>A cache locker thats need to be disposed</returns>
        Task<IDisposable> AcquireLockAsync(string key, TimeSpan timeout);

        /// <summary>
        /// Get value from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>Any value from the cache or default(T)</returns>
        T Get<T>(string key);

        /// <summary>
        /// Retrieves  value from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">Key of the value.</param>
        /// <returns>An instance of <see cref="ConditionalResult{T}"/> where HasValue is True if the value is retrieved from the cache, otherwise false.</returns>
        ConditionalResult<T> TryGet<T>(string key);

        /// <summary>
        /// Get values from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="keys">A list of cache keys</param>
        /// <returns>A Dictionary{key, T} of values from the cache. If the key does not exists in the cache it will not be present in the Dictionary.</returns>
        Dictionary<string, T> Get<T>(IEnumerable<string> keys);

        /// <summary>
        /// Get value from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="key">The cache key</param>
        /// <returns>Any value from the cache or default(T)</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Retrieves  value from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="key">Key of the value.</param>
        /// <returns>An instance of <see cref="ConditionalResult{T}"/> where HasValue is True if the value is retrieved from the cache, otherwise false.</returns>
        Task<ConditionalResult<T>> TryGetAsync<T>(string key);

        /// <summary>
        /// Get values from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="keys">A list of cache keys</param>
        /// <returns>A Dictionary{key, T} of values from the cache. If the key does not exists in the cache it will not be present in the Dictionary.</returns>
        Task<Dictionary<string, T>> GetAsync<T>(IEnumerable<string> keys);

        /// <summary>
        /// Adds or replace a value in the cache.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the value.
        /// </typeparam>
        /// <param name="key">
        /// Key of the value.
        /// </param>
        /// <param name="o">
        /// Value being push in cache.
        /// </param>
        /// <param name="duration">
        /// Duration of the value in the cache.
        /// </param>
        /// <param name="policy">
        /// Priority of the item in the cache.
        /// </param>
        /// <param name="parentKey"> The key of the item that this item is a child of.</param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        void Set<T>(string key, T o, TimeSpan duration, CacheItemPriority policy, string parentKey = null);

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <param name="duration"> Duration of the value in the cache. </param>
        /// <param name="policy"> Priority of the item in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        void Set<T>(IEnumerable<DataToStoreInCache<T>> items, TimeSpan duration, CacheItemPriority policy);

        /// <summary>
        /// Adds or replace a value in the cache.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the value.
        /// </typeparam>
        /// <param name="key">
        /// Key of the value.
        /// </param>
        /// <param name="o">
        /// Value being push in cache.
        /// </param>
        /// <param name="duration">
        /// Duration of the value in the cache.
        /// </param>
        /// <param name="policy">
        /// Priority of the item in the cache.
        /// </param>
        /// <param name="parentKey"> The key of the item that this item is a child of.</param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        /// <returns>A async task</returns>
        Task SetAsync<T>(string key, T o, TimeSpan duration, CacheItemPriority policy, string parentKey = null);

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <param name="duration"> Duration of the value in the cache. </param>
        /// <param name="policy"> Priority of the item in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        /// <returns>A async task</returns>
        Task SetAsync<T>(IEnumerable<DataToStoreInCache<T>> items, TimeSpan duration, CacheItemPriority policy);

        /// <summary>
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="key">
        /// Key of the item that to remove.
        /// </param>
        /// <returns>
        /// True if the item was in the cache, otherwise false.
        /// </returns>
        bool Remove(string key);

        /// <summary>
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="key">
        /// Key of the item that to remove.
        /// </param>
        /// <returns>
        /// True if the item was in the cache, otherwise false.
        /// </returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// Initialize the client with key value settings.
        /// </summary>
        /// <param name="customSettings">
        /// Dictionary containing the settings.
        /// </param>
        void Initialize(IDictionary<string, string> customSettings);

        /// <summary>
        /// Clear the content of this cache.
        /// </summary>
        void Clear();
    }
}
