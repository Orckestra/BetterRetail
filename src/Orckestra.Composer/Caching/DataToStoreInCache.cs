using System;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// Represent a key, its value and its parent for use when setting multiple item in a cache
    /// </summary>
    /// <typeparam name="T">Any reference type</typeparam>
    public class DataToStoreInCache<T>
    {
        /// <summary>
        /// Gets the key used to reference the object from the cache
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the item to be cached
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the parent key
        /// </summary>
        public string ParentKey { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataToStoreInCache{T}"/> class.
        /// </summary>
        /// <param name="key">Cache key of the item</param>
        /// <param name="value">The item to be cached</param>
        /// <param name="parentKey">The key of the item that this item is a child of</param>
        public DataToStoreInCache(string key, T value, string parentKey = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Key = key;
            Value = value;
            ParentKey = parentKey;
        }
    }
}
