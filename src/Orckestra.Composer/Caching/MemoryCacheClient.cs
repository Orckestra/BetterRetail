using System.Collections.Generic;
using System.Runtime.Caching;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// Cache client using in memory caching.
    /// </summary>
    public class MemoryCacheClient : ObjectCacheClient
    {
        public string DefaultName = "Overture";
        public const string CacheNameKey = "Name";

        /// <summary>
        /// Creates a new <see cref="MemoryCache"/> instance.
        /// </summary>
        /// <returns>A new <see cref="MemoryCache"/>.</returns>
        protected override ObjectCache CreateCache()
        {
            return new MemoryCache(Name ?? DefaultName);
        }

        /// <summary>
        /// Gets or sets the cache name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes the specified custom settings.
        /// </summary>
        /// <param name="customSettings">The custom settings.</param>
        public override void Initialize(IDictionary<string, string> customSettings)
        {
            if (customSettings == null)
                return;

            if (customSettings.ContainsKey(CacheNameKey))
            {
                Name = customSettings[CacheNameKey];
            }
        }
    }
}
