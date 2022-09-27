using Orckestra.Composer.Dependency;
using Orckestra.Composer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// Helper class that holds the logic for the GetOrAdd "Many" methods
    /// </summary>
    internal class CacheProviderGetMultipleKeys
    {
        private readonly ICacheClientRegistry _cacheClientRegistry;
        private readonly IDependencyResolver _resolver;

        /// <summary>
        /// Initialize an instance of <see cref="CacheProviderGetMultipleKeys"/>
        /// </summary>
        /// <param name="cacheClientRegistry">The cache client registry instance</param>
        /// <param name="resolver">The dependency resolver instance</param>
        internal CacheProviderGetMultipleKeys(ICacheClientRegistry cacheClientRegistry, IDependencyResolver resolver)
        {
            _cacheClientRegistry = cacheClientRegistry;
            _resolver = resolver;
        }

        internal Dictionary<CacheItemGetDefinition, T> Get<T>(IEnumerable<CacheItemGetDefinition> items)
        {
            Dictionary<CacheItemGetDefinition, T> results = new Dictionary<CacheItemGetDefinition, T>();
            var preparationResult = PrepareParameters<T, Func<CacheItemGetDefinition[], Dictionary<CacheItemGetDefinition, T>>>(items, null, validateGetValues: false);
            if (!preparationResult.HasValue)
            {
                return results;
            }
            var preparedParameters = preparationResult.Value;
            Dictionary<string, T> cachedResults = preparedParameters.CacheClient.Get<T>(preparedParameters.CacheItemGetDefinitions.Select(x => x.GetStoredFullCacheKey()).Distinct());
            List<CacheItemGetDefinition> nonCachedItems;
            AddCachedItemsToResults(cachedResults, results, preparedParameters.CacheItemGetDefinitions, out nonCachedItems);

            return results;
        }

        internal async Task<Dictionary<CacheItemGetDefinition, T>> GetAsync<T>(IEnumerable<CacheItemGetDefinition> items)
        {
            Dictionary<CacheItemGetDefinition, T> results = new Dictionary<CacheItemGetDefinition, T>();
            var preparationResult = PrepareParameters<T, Func<CacheItemGetDefinition[], Task<Dictionary<CacheItemGetDefinition, T>>>>(items, null, validateGetValues: false);
            if (!preparationResult.HasValue)
            {
                return results;
            }
            var preparedParameters = preparationResult.Value;

            Dictionary<string, T> cachedResults = await preparedParameters.CacheClient.GetAsync<T>(preparedParameters.CacheItemGetDefinitions.Select(x => x.GetStoredFullCacheKey()).Distinct()).ConfigureAwait(false);
            List<CacheItemGetDefinition> nonCachedItems;
            AddCachedItemsToResults(cachedResults, results, preparedParameters.CacheItemGetDefinitions, out nonCachedItems);

            return results;
        }

        internal Dictionary<CacheItemGetDefinition, T> GetOrAdd<T>(IEnumerable<CacheItemGetDefinition> items,
                                                                   Func<CacheItemGetDefinition[], Dictionary<CacheItemGetDefinition, T>> getValuesFromSource,
                                                                   Action<Dictionary<CacheItemGetDefinition, T>> setValues = null)
        {
            // This method supports the case where there are multiple CacheItemGetDefinition items with the same FullCacheKey

            Dictionary<CacheItemGetDefinition, T> results = new Dictionary<CacheItemGetDefinition, T>();
            var preparationResult = PrepareParameters<T, Func<CacheItemGetDefinition[], Dictionary<CacheItemGetDefinition, T>>>(items, getValuesFromSource);
            if (!preparationResult.HasValue)
            {
                return results;
            }
            var preparedParameters = preparationResult.Value;

            Dictionary<string, T> cachedResults = preparedParameters.CacheClient.Get<T>(preparedParameters.CacheItemGetDefinitions.Select(x => x.GetStoredFullCacheKey()).Distinct());
            List<CacheItemGetDefinition> nonCachedItems;
            AddCachedItemsToResults(cachedResults, results, preparedParameters.CacheItemGetDefinitions, out nonCachedItems);

            if (nonCachedItems.Count == 0)
            {
                return results;
            }

            var itemsRetrievedFromSource = getValuesFromSource(nonCachedItems.ToArray());
            AddItemsFromSourceToResults(itemsRetrievedFromSource, results, nonCachedItems);

            setValues = setValues ?? (itemsToCache =>
            {
                preparedParameters.CacheClient.Set(ConvertCacheItemDefinitionsToObjectToCacheData(itemsToCache),
                                                   preparedParameters.CachePolicyInfo.Duration,
                                                   preparedParameters.CachePolicyInfo.CacheItemPriority);
            });
            setValues(itemsRetrievedFromSource);

            return results;
        }

        internal async Task<Dictionary<CacheItemGetDefinition, T>> GetOrAddAsync<T>(IEnumerable<CacheItemGetDefinition> items,
                                                                                    Func<CacheItemGetDefinition[], Task<Dictionary<CacheItemGetDefinition, T>>> getValuesFromSource,
                                                                                    Func<Dictionary<CacheItemGetDefinition, T>, Task> setValues = null)
        {
            // This method supports the case where there are multiple CacheItemGetDefinition items with the same FullCacheKey

            Dictionary<CacheItemGetDefinition, T> results = new Dictionary<CacheItemGetDefinition, T>();
            var preparationResult = PrepareParameters<T, Func<CacheItemGetDefinition[], Task<Dictionary<CacheItemGetDefinition, T>>>>(items, getValuesFromSource);
            if (!preparationResult.HasValue)
            {
                return results;
            }
            var preparedParameters = preparationResult.Value;

            Dictionary<string, T> cachedResults = await preparedParameters.CacheClient.GetAsync<T>(preparedParameters.CacheItemGetDefinitions.Select(x => x.GetStoredFullCacheKey()).Distinct()).ConfigureAwait(false);
            List<CacheItemGetDefinition> nonCachedItems;
            AddCachedItemsToResults(cachedResults, results, preparedParameters.CacheItemGetDefinitions, out nonCachedItems);

            if (nonCachedItems.Count == 0)
            {
                return results;
            }

            var itemsRetrievedFromSource = await getValuesFromSource(nonCachedItems.ToArray()).ConfigureAwait(false);
            AddItemsFromSourceToResults(itemsRetrievedFromSource, results, nonCachedItems);

            setValues = setValues ?? (async itemsToCache =>
            {
                await preparedParameters.CacheClient.SetAsync(ConvertCacheItemDefinitionsToObjectToCacheData(itemsToCache),
                                                              preparedParameters.CachePolicyInfo.Duration,
                                                              preparedParameters.CachePolicyInfo.CacheItemPriority)
                                        .ConfigureAwait(false);
            });
            await setValues(itemsRetrievedFromSource).ConfigureAwait(false);

            return results;
        }

        private ConditionalResult<PreparedParameters> PrepareParameters<T, TGetValues>(IEnumerable<CacheItemGetDefinition> items, TGetValues getValuesFromSource, bool validateGetValues = true)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            if (validateGetValues && getValuesFromSource == null)
            {
                throw new ArgumentNullException("getValuesFromSource");
            }

            PreparedParameters result = new PreparedParameters
            {
                CacheItemGetDefinitions = items as CacheItemGetDefinition[] ?? items.ToArray()
            };

            ValidateCacheItemGetDefinition(result.CacheItemGetDefinitions);

            if (result.CacheItemGetDefinitions.Length == 0)
            {
                return new ConditionalResult<PreparedParameters>();
            }

            result.CachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(result.CacheItemGetDefinitions.First().Key, true);
            result.CacheClient = result.CachePolicyInfo.CreateClient(_resolver);
            return new ConditionalResult<PreparedParameters>(result);
        }

        private IEnumerable<DataToStoreInCache<T>> ConvertCacheItemDefinitionsToObjectToCacheData<T>(Dictionary<CacheItemGetDefinition, T> itemsToCache)
        {
            return itemsToCache.Select(kvp => new DataToStoreInCache<T>(kvp.Key.GetStoredFullCacheKey(),
                                                                        kvp.Value,
                                                                        parentKey: kvp.Key.GetStoredParentFullCacheKey()));
        }

        private void AddItemsFromSourceToResults<T>(Dictionary<CacheItemGetDefinition, T> itemsRetrievedFromSource,
                                                    Dictionary<CacheItemGetDefinition, T> results,
                                                    List<CacheItemGetDefinition> cacheItemNotCached)
        {
            foreach (KeyValuePair<CacheItemGetDefinition, T> item in itemsRetrievedFromSource)
            {
                foreach (var nonCachedItem in cacheItemNotCached)
                {
                    if (nonCachedItem.GetStoredFullCacheKey() == item.Key.GetStoredFullCacheKey())
                    {
                        results.Add(nonCachedItem, item.Value);
                    }
                }
            }
        }

        private void ValidateCacheItemGetDefinition(CacheItemGetDefinition[] cacheItemGetDefinitions)
        {
            if (cacheItemGetDefinitions.Length > 0)
            {
                var categoryName = cacheItemGetDefinitions[0].Key.CategoryName;
                if (cacheItemGetDefinitions.Any(x => x.Key.CategoryName != categoryName))
                {
                    throw new ArgumentException("Parameter items cannot contain CacheKeys from more than one category");
                }
            }

            foreach (var cacheItemDefinition in cacheItemGetDefinitions)
            {
                cacheItemDefinition.ClearStoredKeys();
            }
        }

        /// <summary>
        /// Adds the <paramref name="cachedItems"/> to <paramref name="results"/> if they are present in <paramref name="itemsNotPresentInResults"/>.
        /// They are added to <paramref name="itemsNotFound"/> if they are not present.
        /// </summary>
        /// <typeparam name="T">Any reference types</typeparam>
        /// <param name="cachedItems">Objects that comes from the cache</param>
        /// <param name="results">Objects taht contains the values that will be returned by GetOrAdd</param>
        /// <param name="itemsNotPresentInResults"></param>
        /// <param name="itemsNotFound">Objects from <paramref name="itemsNotPresentInResults"/> that were not found in <paramref name="cachedItems"/></param>
        private void AddCachedItemsToResults<T>(Dictionary<string, T> cachedItems,
                                                Dictionary<CacheItemGetDefinition, T> results,
                                                IEnumerable<CacheItemGetDefinition> itemsNotPresentInResults,
                                                out List<CacheItemGetDefinition> itemsNotFound)
        {
            itemsNotFound = new List<CacheItemGetDefinition>();

            foreach (var cacheItemDefinition in itemsNotPresentInResults)
            {
                T obj;
                if (cachedItems.TryGetValue(cacheItemDefinition.GetStoredFullCacheKey(), out obj))
                {
                    results.Add(cacheItemDefinition, obj);
                }
                else
                {
                    itemsNotFound.Add(cacheItemDefinition);
                }
            }
        }

        /// <summary>
        /// Small class that holds the validated/parsed parameters of the GetOrAdd methods
        /// </summary>
        private class PreparedParameters
        {
            public CacheItemGetDefinition[] CacheItemGetDefinitions { get; set; }
            public CacheItemPolicyInfo CachePolicyInfo { get; set; }
            public ICacheClient CacheClient { get; set; }
        }
    }
}
