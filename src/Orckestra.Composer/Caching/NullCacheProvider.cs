using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    public class NullCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Retrieves  value from the cache.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="cacheKey">Key of the value.</param>
        /// <param name="value">Value retrieved from the cached.</param>
        /// <returns>True if the value is retrieved from the cache, otherwise false.</returns>
        public bool TryGet<T>(CacheKey cacheKey, out T value)
        {
            value = default(T);
            return false;
        }

        public T Get<T>(CacheKey cacheKey)
        {
            return default(T);
        }

        public Task<T> GetAsync<T>(CacheKey cacheKey)
        {
            return Task.FromResult(default(T));
        }

        public Dictionary<CacheItemGetDefinition, T> Get<T>(IEnumerable<CacheItemGetDefinition> items)
        {
            return new Dictionary<CacheItemGetDefinition, T>();
        }

        public Task<Dictionary<CacheItemGetDefinition, T>> GetAsync<T>(IEnumerable<CacheItemGetDefinition> items)
        {
            return Task.FromResult(new Dictionary<CacheItemGetDefinition, T>());
        }

        /// <summary>
        /// Replace a value from the cache. If the value doesn't exist, an exception will be fired.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="cacheKey">key of the item.</param>
        /// <param name="o">Value.</param>
        /// <param name="parentKey"></param>
        public void Set<T>(CacheKey cacheKey, T o, CacheKey parentKey = null)
        {

        }

        public Task SetAsync<T>(CacheKey cacheKey, T o, CacheKey parentKey = null)
        {
            return Task.FromResult(false);
        }

        public void Set<T>(IEnumerable<CacheItemSetDefinition<T>> items)
        {

        }

        public Task SetAsync<T>(IEnumerable<CacheItemSetDefinition<T>> items)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Replace a value from the cache. If the value doesn't exist, an exception will be fired.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="cacheKey">key of the item.</param>
        /// <param name="o">Value.</param>
        public void Replace<T>(CacheKey cacheKey, T o)
        {
        }

        public Task ReplaceAsync<T>(CacheKey cacheKey, T o)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Adds a new value in the cache. If the value already exists, an exception will be fired.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="cacheKey">key of the item.</param>
        /// <param name="o">Value.</param>
        /// <param name="parentKey"></param>
        public void Add<T>(CacheKey cacheKey, T o, CacheKey parentKey = null)
        {
        }

        public Task AddAsync<T>(CacheKey cacheKey, T o, CacheKey parentKey = null)
        {
            return Task.FromResult(false);
        }

        public T GetOrAdd<T>(CacheKey cacheKey, Func<T> getValueFromSource, Action<T> setValue = null,
            CacheKey parentKey = null)
        {
            return getValueFromSource();
        }

        public Dictionary<CacheItemGetDefinition, T> GetOrAdd<T>(IEnumerable<CacheItemGetDefinition> items,
            Func<CacheItemGetDefinition[], Dictionary<CacheItemGetDefinition, T>> getValuesFromSource,
            Action<Dictionary<CacheItemGetDefinition, T>> setValues = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (getValuesFromSource == null)
            {
                throw new ArgumentNullException("getValuesFromSource");
            }

            var itemsToGetFromSource = (items as CacheItemGetDefinition[]) ?? items.ToArray();
            return getValuesFromSource(itemsToGetFromSource);
        }

        public Task<T> GetOrAddAsync<T>(CacheKey cacheKey, Func<Task<T>> getValueFromSourceAsync,
            Func<T, Task> setValueAsync = null,
            CacheKey parentKey = null)
        {
            return getValueFromSourceAsync();
        }

        public Task<Dictionary<CacheItemGetDefinition, T>> GetOrAddAsync<T>(IEnumerable<CacheItemGetDefinition> items,
            Func<CacheItemGetDefinition[], Task<Dictionary<CacheItemGetDefinition, T>>> getValuesFromSource,
            Func<Dictionary<CacheItemGetDefinition, T>, Task> setValues = null)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (getValuesFromSource == null)
            {
                throw new ArgumentNullException("getValuesFromSource");
            }

            var itemsToGetFromSource = (items as CacheItemGetDefinition[]) ?? items.ToArray();
            return getValuesFromSource(itemsToGetFromSource);
        }

        /// <summary>
        /// Removes a value in the cache.
        /// </summary>
        /// <param name="cacheKey">Key of the value.</param>
        /// <returns>True if the item were in the cache, otherwise false.</returns>
        public bool Remove(CacheKey cacheKey)
        {
            return false;
        }

        public Task<bool> RemoveAsync(CacheKey cacheKey)
        {
            return Task.FromResult(false);
        }
    }
}
