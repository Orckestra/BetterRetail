using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    /// Provides implementations for some common <see cref="ICacheClient"/> methods.
    /// </summary>
    internal static class CacheClientHelper
    {
        /// <summary>
        /// Calls the Get{T} method for each keys
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static Dictionary<string, T> GetSequential<T>(ICacheClient client, IEnumerable<string> keys)
        {
            var results = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var result = client.TryGet<T>(key);
                if (result.HasValue)
                {
                    results.Add(key, result.Value);
                }
            }
            return results;
        }

        /// <summary>
        /// Provide a common implementation of Get{T} for ICacheClients that uses TryGet{T} and returns defaulT(T) if TryGet{T} returns false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(ICacheClient client, string key)
        {
            var result = client.TryGet<T>(key);
            if (result.HasValue)
            {
                return result.Value;
            }
            return default(T);
        }

        /// <summary> Adds or replace a list of values in the cache. </summary>
        /// <typeparam name="T"> Type of the value. </typeparam>
        /// <param name="client">The instance of <see cref="ICacheClient"/> that will be used to set the object in the cache</param>
        /// <param name="items"> List of items to add or remplace in the cache. </param>
        /// <param name="duration"> Duration of the value in the cache. </param>
        /// <param name="policy"> Priority of the item in the cache. </param>
        /// <exception cref="System.ArgumentException">Provided key, value, duration or policy are null.</exception>
        public static void Set<T>(ICacheClient client, IEnumerable<DataToStoreInCache<T>> items, TimeSpan duration, CacheItemPriority policy)
        {
            foreach (var item in items)
            {
                client.Set(item.Key, item.Value, duration, policy, item.ParentKey);
            }
        }
    }
}
