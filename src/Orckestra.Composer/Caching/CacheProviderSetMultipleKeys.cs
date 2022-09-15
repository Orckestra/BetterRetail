using Orckestra.Composer.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// Helper class that holds the logic for the Set "Many" methods
    /// </summary>
    internal class CacheProviderSetMultipleKeys
    {
        private readonly ICacheClientRegistry _cacheClientRegistry;
        private readonly IDependencyResolver _resolver;

        /// <summary>
        /// Initialize an instance of <see cref="CacheProviderGetMultipleKeys"/>
        /// </summary>
        /// <param name="cacheClientRegistry">The cache client registry instance</param>
        /// <param name="resolver">The dependency resolver instance</param>
        public CacheProviderSetMultipleKeys(ICacheClientRegistry cacheClientRegistry, IDependencyResolver resolver)
        {
            _cacheClientRegistry = cacheClientRegistry;
            _resolver = resolver;
        }

        public void Set<T>(IEnumerable<CacheItemSetDefinition<T>> items)
        {
            if (items == null) { throw new ArgumentNullException("items"); }

            var values = new MultipleSetValues<T>(items);
            if (!values.HasValues)
            {
                return;
            }

            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(values.CacheItemSetDefinitions[0].Key, true);
            var cacheClient = cachePolicyInfo.CreateClient(_resolver);

            cacheClient.Set(values.ValuesToStore, cachePolicyInfo.Duration, cachePolicyInfo.CacheItemPriority);
        }

        public Task SetAsync<T>(IEnumerable<CacheItemSetDefinition<T>> items)
        {
            if (items == null) { throw new ArgumentNullException("items"); }

            var values = new MultipleSetValues<T>(items);
            if (!values.HasValues)
            {
                return Task.FromResult(true);
            }

            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(values.CacheItemSetDefinitions[0].Key, true);
            var cacheClient = cachePolicyInfo.CreateClient(_resolver);

            return cacheClient.SetAsync(values.ValuesToStore, cachePolicyInfo.Duration, cachePolicyInfo.CacheItemPriority);
        }

        private class MultipleSetValues<T>
        {
            public MultipleSetValues(IEnumerable<CacheItemSetDefinition<T>> items)
            {
                CacheItemSetDefinitions = items as CacheItemSetDefinition<T>[] ?? items.ToArray();
                ValuesToStore = CacheItemSetDefinitions.Select(x => new DataToStoreInCache<T>(x.Key.GetFullCacheKey(),
                                                                            x.Value,
                                                                            parentKey: x.GetStoredParentFullCacheKey()))
                                     .ToArray();

            }

            public CacheItemSetDefinition<T>[] CacheItemSetDefinitions { get; set; }
            public DataToStoreInCache<T>[] ValuesToStore { get; set; }
            public bool HasValues { get { return ValuesToStore != null && ValuesToStore.Length > 0; } }
        }
    }
}
