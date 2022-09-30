using System;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// Represent a key and its parent for use when querying the cache for multiple items at the same time
    /// </summary>
    /// <typeparam name="T">Any reference type</typeparam>
    public class CacheItemSetDefinition<T> : CacheItemDefinitionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemSetDefinition{T}"/> class.
        /// </summary>
        /// <param name="key">Cache key of the item</param>
        /// <param name="value">The value to be stored in the cache</param>
        /// <param name="parentKey">The key of the item that this item is a child of</param>
        public CacheItemSetDefinition(CacheKey key, T value, CacheKey parentKey = null)
            : base(key, parentKey)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value of the item to be cached
        /// </summary>
        public T Value { get; private set; }
    }


    /// <summary>
    /// Represent a key and its parent for use when querying the cache for multiple items at the same time
    /// </summary>
    public class CacheItemGetDefinition : CacheItemDefinitionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemGetDefinition"/> class.
        /// </summary>
        /// <param name="key">Cache key of the item</param>
        /// <param name="parentKey">The key of the item that this item is a child of</param>
        /// <param name="cargo">The value that is used to identify the object when retrieving it from it source. E.g.: It can be the primary key used to query the database for a specify object.</param>
        public CacheItemGetDefinition(CacheKey key, CacheKey parentKey = null, object cargo = null)
            : base(key, parentKey)
        {
            Cargo = cargo;
        }

        /// <summary>
        /// Gets an optional additional value that is used when retrieving an object from its source. E.g.: It can be the primary key used to query the database for a specify object.
        /// </summary>
        public object Cargo { get; private set; }
    }

    /// <summary>
    /// Represents the base class for CacheItemDefinition items
    /// </summary>
    public abstract class CacheItemDefinitionBase
    {
        /// <summary>
        /// Gets the key used to retrieve the object from the cache
        /// </summary>
        public CacheKey Key { get; private set; }

        /// <summary>
        /// Gets the key of the item that this item is a child of
        /// </summary>
        public CacheKey ParentKey { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemDefinitionBase"/> class.
        /// </summary>
        /// <param name="key">Cache key of the item</param>
        /// <param name="parentKey">The key of the item that this item is a child of</param>
        protected CacheItemDefinitionBase(CacheKey key, CacheKey parentKey = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Key = key;
            ParentKey = parentKey;
        }

        private string _cachedParentFullCacheKey = null;
        private string _cachedFullCacheKey = null;


        /// <summary>
        /// Keeps a copy of the <see cref="ParentKey"/>'s full cache key in a local variable to prevent computing it every times.
        /// </summary>
        /// <returns>The complete cache key of ParentKey.</returns>
        /// <remarks>You need to call <see cref="ClearStoredKeys"/> if you make any changes to the <see cref="ParentKey"/></remarks>
        public string GetStoredParentFullCacheKey()
        {
            return _cachedParentFullCacheKey ?? (_cachedParentFullCacheKey = ParentKey != null ? ParentKey.GetFullCacheKey() : null);
        }

        /// <summary>
        /// Keeps a copy of the <see cref="Key"/>'s full cache key in a local variable to prevent computing it every times.
        /// </summary>
        /// <returns>The complete cache key of Key.</returns>
        /// <remarks>You need to call <see cref="ClearStoredKeys"/> if you make any changes to the <see cref="Key"/></remarks>
        public string GetStoredFullCacheKey()
        {
            return _cachedFullCacheKey ?? (_cachedFullCacheKey = Key.GetFullCacheKey());
        }

        /// <summary>
        /// Resets the stored cache keys to their default values;
        /// </summary>
        public void ClearStoredKeys()
        {
            _cachedParentFullCacheKey = null;
            _cachedFullCacheKey = null;
        }
    }
}
