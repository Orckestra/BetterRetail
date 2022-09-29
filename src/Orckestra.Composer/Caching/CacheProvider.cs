using Orckestra.Composer.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Profiler = Orckestra.Composer.Dependency.Profiler;

namespace Orckestra.Composer.Caching
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IDependencyResolver _resolver;
        private readonly ICacheClientRegistry _cacheClientRegistry;

        public CacheProvider(IDependencyResolver resolver, ICacheClientRegistry cacheClientRegistry)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (cacheClientRegistry == null) throw new ArgumentNullException(nameof(cacheClientRegistry));

            _resolver = resolver;
            _cacheClientRegistry = cacheClientRegistry;
        }

        public T Get<T>(CacheKey cacheKey)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);
            return client.Get<T>(cacheKey.GetFullCacheKey());
        }

        public Task<T> GetAsync<T>(CacheKey cacheKey)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);
            return client.GetAsync<T>(cacheKey.GetFullCacheKey());
        }

        public Dictionary<CacheItemGetDefinition, T> Get<T>(IEnumerable<CacheItemGetDefinition> items)
        {
            var multipleKeyGetter = new CacheProviderGetMultipleKeys(_cacheClientRegistry, _resolver);
            return multipleKeyGetter.Get<T>(items);
        }

        public Task<Dictionary<CacheItemGetDefinition, T>> GetAsync<T>(IEnumerable<CacheItemGetDefinition> items)
        {
            var multipleKeyGetter = new CacheProviderGetMultipleKeys(_cacheClientRegistry, _resolver);
            return multipleKeyGetter.GetAsync<T>(items);
        }

        public void Set<T>(CacheKey cacheKey, T o, CacheKey parentKey = null)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);

            var pKey = parentKey?.GetFullCacheKey();
            client.Set(cacheKey.GetFullCacheKey(), o, cachePolicyInfo.Duration, cachePolicyInfo.CacheItemPriority, pKey);
        }

        public Task SetAsync<T>(CacheKey cacheKey, T o, CacheKey parentKey = null)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);

            var pKey = parentKey?.GetFullCacheKey();
            return client.SetAsync(cacheKey.GetFullCacheKey(), o, cachePolicyInfo.Duration, cachePolicyInfo.CacheItemPriority, pKey);
        }

        public void Set<T>(IEnumerable<CacheItemSetDefinition<T>> items)
        {
            var multipleKeySetter = new CacheProviderSetMultipleKeys(_cacheClientRegistry, _resolver);
            multipleKeySetter.Set(items);
        }

        public Task SetAsync<T>(IEnumerable<CacheItemSetDefinition<T>> items)
        {
            var multipleKeySetter = new CacheProviderSetMultipleKeys(_cacheClientRegistry, _resolver);
            return multipleKeySetter.SetAsync(items);
        }

        public T GetOrAdd<T>(CacheKey cacheKey, Func<T> getValueFromSource, Action<T> setValue = null, CacheKey parentKey = null)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);
            var key = cacheKey.GetFullCacheKey();
            var pKey = parentKey?.GetFullCacheKey();

            //If current cache client implements ICacheClientV2 interface, use the new GetOrAdd
            var cacheClientV2 = client as ICacheClientV2;
            if (cacheClientV2 != null)
                return cacheClientV2.GetOrAdd(key, getValueFromSource, cachePolicyInfo.Duration);

            var val = client.Get<T>(key);
            if (val != null)
            {
                return val;
            }

            using (Profiler.Current.Step(
                $"Cache miss, category '{cacheKey.CategoryName}', client '{client.GetType().Name}'", $"Key: '{key}'"))
            using (client.AcquireLock(key, cachePolicyInfo.AcquiredLockTimeout))
            {
                val = client.Get<T>(key);
                if (val != null)
                {
                    using (Profiler.Current.Step("Returning cached value"))
                    {
                        return val;
                    }
                }

                using (Profiler.Current.Step("Getting value from source"))
                {
                    val = getValueFromSource();
                }

                if (val == null)
                {
                    return default(T);
                }
                setValue = setValue ?? (v => client.Set(key, v, cachePolicyInfo.Duration, cachePolicyInfo.CacheItemPriority, pKey));
                setValue(val);

                return val;
            }
        }

        public Dictionary<CacheItemGetDefinition, T> GetOrAdd<T>(IEnumerable<CacheItemGetDefinition> items,
                                                                 Func<CacheItemGetDefinition[], Dictionary<CacheItemGetDefinition, T>> getValuesFromSource,
                                                                 Action<Dictionary<CacheItemGetDefinition, T>> setValues = null)
        {
            var multipleKeyGetter = new CacheProviderGetMultipleKeys(_cacheClientRegistry, _resolver);
            return multipleKeyGetter.GetOrAdd(items, getValuesFromSource, setValues: setValues);
        }

        public async Task<T> GetOrAddAsync<T>(CacheKey cacheKey, Func<Task<T>> getValueFromSourceAsync, Func<T, Task> setValueAsync = null, CacheKey parentKey = null)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);
            var key = cacheKey.GetFullCacheKey();
            var pKey = parentKey?.GetFullCacheKey();

            var val = await client.GetAsync<T>(key).ConfigureAwait(false);

            if (val != null)
            {
                return val;
            }

            using (Profiler.Current.Step(
                $"Cache miss, category '{cacheKey.CategoryName}', client '{client.GetType().Name}'", $"Key: '{key}'"))
            using (await client.AcquireLockAsync(key, cachePolicyInfo.AcquiredLockTimeout).ConfigureAwait(false))
            {
                val = await client.GetAsync<T>(key).ConfigureAwait(false);
                if (val != null)
                {
                    return val;
                }

                using (Profiler.Current.Step("Getting value from source"))
                {
                    val = await getValueFromSourceAsync().ConfigureAwait(false);
                }

                if (val == null)
                {
                    return default(T);
                }

                setValueAsync = setValueAsync ?? (v => client.SetAsync(key, v, cachePolicyInfo.Duration, cachePolicyInfo.CacheItemPriority, pKey));
                await setValueAsync(val).ConfigureAwait(false);

                return val;
            }
        }

        public Task<Dictionary<CacheItemGetDefinition, T>> GetOrAddAsync<T>(IEnumerable<CacheItemGetDefinition> items,
                                                                            Func<CacheItemGetDefinition[], Task<Dictionary<CacheItemGetDefinition, T>>> getValuesFromSource,
                                                                            Func<Dictionary<CacheItemGetDefinition, T>, Task> setValues = null)
        {
            var multipleKeyGetter = new CacheProviderGetMultipleKeys(_cacheClientRegistry, _resolver);
            return multipleKeyGetter.GetOrAddAsync(items, getValuesFromSource, setValues: setValues);
        }

        public bool Remove(CacheKey cacheKey)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);
            return client.Remove(cacheKey.GetFullCacheKey());
        }

        public Task<bool> RemoveAsync(CacheKey cacheKey)
        {
            var cachePolicyInfo = _cacheClientRegistry.GetCacheItemPolicyInfo(cacheKey, true);
            var client = cachePolicyInfo.CreateClient(_resolver);
            return client.RemoveAsync(cacheKey.GetFullCacheKey());
        }

        public void ApplyConfiguration(CacheConfiguration instance)
        {
            _cacheClientRegistry.ApplyConfiguration(instance);
        }
    }
}

