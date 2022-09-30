using System;
using System.Globalization;
using System.Text;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    ///     Represent the master key of an entity.
    /// </summary>
    public class EntityCacheKey : CacheKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCacheKey"/> class. 
        /// </summary>
        /// <param name="categoryName">
        /// Name of the category.
        /// </param>
        /// <param name="scope">Id of the scope</param>
        /// <param name="entityType">Type of the entity</param>
        /// <param name="entityId">Id of the entity</param>
        public EntityCacheKey(string categoryName, string scope, string entityType, string entityId)
            : base(categoryName)
        {
            Scope = scope;
            AppendKeyParts(entityType, entityId);
        }

        /// <summary>
        /// Create a related key of the current master key
        /// </summary>
        /// <param name="childKeyParts">Key parts</param>
        /// <returns>A new related cache key</returns>
        public CacheKey CreateRelatedKey(params object[] childKeyParts)
        {
            var childKey = new CacheKey(CategoryName)
            {
                Scope = Scope,
                Key = Key
            };
            childKey.AppendKeyParts(childKeyParts);

            return childKey;
        }
    }

    /// <summary>
    ///     Represent the key of an entry in the cache system.
    /// </summary>
    public class CacheKey
    {
        /// <summary>
        ///     The delimiter used when joining each key part in the system.
        /// </summary>
        private const string Delimiter = ":";

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKey"/> class. 
        /// </summary>
        /// <param name="categoryName">
        /// Name of the category.
        /// </param>
        public CacheKey(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("categoryName");
            }

            CategoryName = categoryName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKey"/> class. 
        /// </summary>
        /// <param name="categoryName">
        /// Name of the category.
        /// </param>
        /// <param name="key">
        /// Key of the item to be put in cache.
        /// </param>
        public CacheKey(string categoryName, string key)
            : this(categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("categoryName");
            }

            CategoryName = categoryName;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKey"/> class. 
        /// Creates a new instance of <see cref="CacheKey"/>.
        /// </summary>
        /// <param name="categoryName">
        /// Name of the category.
        /// </param>
        /// <param name="key">
        /// Key of the item to be put in cache.
        /// </param>
        /// <param name="cultureInfo">
        /// Culture associated with the item put in cache.
        /// </param>
        public CacheKey(string categoryName, string key, CultureInfo cultureInfo)
            : this(categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("categoryName");
            }

            CategoryName = categoryName;
            Key = key;
            CultureInfo = cultureInfo;
        }

        /// <summary>
        ///     Gets the name of the cache category of the entry.
        /// </summary>
        public string CategoryName { get; private set; }

        /// <summary>
        ///     Gets or sets the specific key of the item.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or sets the scope id of the item.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        ///     Gets or sets the culture associated with the entry.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Appends and joins new parts of the <see cref="Key"/> using the set constant <seealso cref="Delimiter"/>.
        /// </summary>
        /// <param name="parts">
        /// New parts to be added to the <see cref="Key"/>.
        /// </param>
        public void AppendKeyParts(params object[] parts)
        {
            if (!string.IsNullOrWhiteSpace(Key))
            {
                Key += Delimiter;
            }

            Key += string.Join(Delimiter, parts);
        }

        /// <summary>
        /// Sets and joins parts of the key.
        /// </summary>
        /// <param name="parts">
        /// Parts that should be used to compose the <see cref="Key"/>.
        /// </param>
        public void SetKeys(params object[] parts)
        {
            Key = string.Join(Delimiter, parts);
        }

        /// <summary>
        ///     Build and return a complete cache key in the following format:
        ///     [CategoryName]_[Scope]_[Key]_[CultureInfo.Name].
        /// </summary>
        /// <returns>The complete cache key.</returns>
        public virtual string GetFullCacheKey()
        {
            var stringBuilder = new StringBuilder(CategoryName);

            if (!string.IsNullOrWhiteSpace(Scope))
            {
                stringBuilder.Append(Delimiter + Scope);
            }

            if (!string.IsNullOrWhiteSpace(Key))
            {
                stringBuilder.Append(Delimiter + Key);
            }

            if (CultureInfo != null)
            {
                stringBuilder.Append(Delimiter + CultureInfo.Name);
            }

            return stringBuilder.ToString();
        }

        public static implicit operator string(CacheKey cacheKey) => cacheKey.GetFullCacheKey();
    }
}
