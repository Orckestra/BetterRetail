using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    ///     This interface defines the contract that must be implemented by all caching providers.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Get value from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="cacheKey">The cache key</param>
        /// <returns>Any value from the cache or default(T)</returns>
        T Get<T>(CacheKey cacheKey);

        /// <summary>
        /// Get values from cache
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="items"> List of items to get from the cache. </param>
        /// <returns>Any value from the cache or default(T)</returns>
        Dictionary<CacheItemGetDefinition, T> Get<T>(IEnumerable<CacheItemGetDefinition> items);

        /// <summary>
        /// Get values from cache
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="items"> List of items to get from the cache. </param>
        /// <returns>Any value from the cache or default(T)</returns>
        Task<Dictionary<CacheItemGetDefinition, T>> GetAsync<T>(IEnumerable<CacheItemGetDefinition> items);


        /// <summary>
        /// Get value from cache or default to T
        /// </summary>
        /// <typeparam name="T">Any reference type</typeparam>
        /// <param name="cacheKey">The cache key</param>
        /// <returns>Any value from the cache or default(T)</returns>
        Task<T> GetAsync<T>(CacheKey cacheKey);

        /// <summary>
        /// Replace or sets a value of an item in the cache.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the value.
        /// </typeparam>
        /// <param name="cacheKey">
        /// <see cref="CacheKey"/> of the item.
        /// </param>
        /// <param name="o">
        /// Value of the item to be set in cache.
        /// </param>
        /// <param name="parentKey"> The key of the item that this item is a child of.</param>
        void Set<T>(CacheKey cacheKey, T o, CacheKey parentKey = null);

        /// <summary>
        /// Replace or sets a value of an item in the cache.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the value.
        /// </typeparam>
        /// <param name="cacheKey">
        /// <see cref="CacheKey"/> of the item.
        /// </param>
        /// <param name="o">
        /// Value of the item to be set in cache.
        /// </param>
        /// <param name="parentKey"> The key of the item that this item is a child of.</param>
        /// <returns>A async task</returns>
        Task SetAsync<T>(CacheKey cacheKey, T o, CacheKey parentKey = null);

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        void Set<T>(IEnumerable<CacheItemSetDefinition<T>> items);

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        /// <returns>A async task</returns>
        Task SetAsync<T>(IEnumerable<CacheItemSetDefinition<T>> items);

        /// <summary>
        /// Get a value from cache or create it using a distributed locked provide by the underlying client
        /// </summary>
        /// <typeparam name="T">Any nullable value type or reference type</typeparam>
        /// <param name="cacheKey">The cache key</param>
        /// <param name="getValueFromSource">Function to retreive the value in case of cache miss</param>
        /// <param name="setValue">Action to be provided if the caller require to set multiple values during the distributed lock</param>
        /// <param name="parentKey">The parent key to provide for cache dependency</param>
        /// <returns>Any value from the cache or default(T)</returns>
        T GetOrAdd<T>(CacheKey cacheKey, Func<T> getValueFromSource, Action<T> setValue = null, CacheKey parentKey = null);

        /// <summary>
        /// Get values from cache or create it using a distributed locked provide by the underlying client
        /// </summary>
        /// <typeparam name="T">Any nullable value type or reference type</typeparam>
        /// <param name="items">List of items to be retrieved from the cache</param>
        /// <param name="getValuesFromSource">Function to retreive the value in case of cache miss</param>
        /// <param name="setValues">Action to be provided if the caller require to set multiple values during the distributed lock</param>
        /// <returns>A dictionary that contains the items found in the cache or from getValuesFromSource</returns>
        Dictionary<CacheItemGetDefinition, T> GetOrAdd<T>(IEnumerable<CacheItemGetDefinition> items, Func<CacheItemGetDefinition[], Dictionary<CacheItemGetDefinition, T>> getValuesFromSource, Action<Dictionary<CacheItemGetDefinition, T>> setValues = null);

        /// <summary>
        /// Get a value from cache or create it using a distributed locked provide by the underlying client
        /// </summary>
        /// <typeparam name="T">Any nullable value type or reference type</typeparam>
        /// <param name="cacheKey">The cache key</param>
        /// <param name="getValueFromSourceAsync">Function to retreive the value in case of cache miss</param>
        /// <param name="setValueAsync">Action to be provided if the caller require to set multiple values during the distributed lock</param>
        /// <param name="parentKey">The parent key to provide for cache dependency</param>
        /// <returns>Any value from the cache or default(T)</returns>
        Task<T> GetOrAddAsync<T>(CacheKey cacheKey, Func<Task<T>> getValueFromSourceAsync, Func<T, Task> setValueAsync = null, CacheKey parentKey = null);

        /// <summary>
        /// Get values from cache or create it using a distributed locked provide by the underlying client
        /// </summary>
        /// <typeparam name="T">Any nullable value type or reference type</typeparam>
        /// <param name="items">List of items to be retrieved from the cache</param>
        /// <param name="getValuesFromSource">Function to retreive the value in case of cache miss</param>
        /// <param name="setValues">Action to be provided if the caller require to set multiple values during the distributed lock</param>
        /// <returns>A dictionary that contains the items found in the cache or from getValuesFromSource</returns>
        Task<Dictionary<CacheItemGetDefinition, T>> GetOrAddAsync<T>(IEnumerable<CacheItemGetDefinition> items, Func<CacheItemGetDefinition[], Task<Dictionary<CacheItemGetDefinition, T>>> getValuesFromSource, Func<Dictionary<CacheItemGetDefinition, T>, Task> setValues = null);

        /// <summary>
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="cacheKey">
        /// <see cref="CacheKey"/> of the item.
        /// </param>
        /// <returns>
        /// True if the item was in the cache, otherwise false.
        /// </returns>
        bool Remove(CacheKey cacheKey);

        /// <summary>
        /// Removes an item in the cache.
        /// </summary>
        /// <param name="cacheKey">
        /// <see cref="CacheKey"/> of the item.
        /// </param>
        /// <returns>
        /// True if the item was in the cache, otherwise false.
        /// </returns>
        Task<bool> RemoveAsync(CacheKey cacheKey);
    }
}
