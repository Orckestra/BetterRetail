using System;
using System.Collections.Concurrent;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// A basic in memory cache object. 
    /// </summary>
    /// <typeparam name="TKey">Type of the key of an object in the cache.</typeparam>
    /// <typeparam name="TValue">Type of the value store.</typeparam>
    public class MemoryCache<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _getValue;
        private readonly Func<TKey, string> _getKey;

        private readonly ConcurrentDictionary<string, TValue> _cache = new ConcurrentDictionary<string, TValue>();


        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="getValue">Method to retrieve value.</param>
        /// <param name="getKey">Method to generate the key from the entity.</param>
        /// <exception cref="System.ArgumentNullException">When a parameter is null.</exception>
        public MemoryCache(Func<TKey, TValue> getValue, Func<TKey, string> getKey)
        {
            if (getValue == null) throw new ArgumentNullException("getValue");
            if (getKey == null) throw new ArgumentNullException("getKey");

            _getValue = getValue;
            _getKey = getKey;
        }

        /// <summary>
        /// Gets the item associated with the key..
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns>The value.</returns>
        public TValue GetItem(TKey key)
        {
            return _cache.GetOrAdd(_getKey(key), k => _getValue(key));
        }
    }
}
