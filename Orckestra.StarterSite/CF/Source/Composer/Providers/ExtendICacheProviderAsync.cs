using System;
using System.Threading.Tasks;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Extend the ICache interface with some useful method implementation  
    /// </summary>
    public static class ExtendICacheProviderAsync
    {
        /// <summary>
        /// Invalidate the current Cache and Set and Get the new one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheProvider"></param>
        /// <param name="cacheKey"></param>
        /// <param name="fromSource"></param>
        /// <returns></returns>
        public static async Task<T> ExecuteAndSetAsync<T>(this ICacheProvider cacheProvider, CacheKey cacheKey, Func<Task<T>> fromSource)
        {
            if (cacheKey == null) { throw new ArgumentNullException(nameof(cacheKey)); }
            if (fromSource == null) { throw new ArgumentNullException(nameof(fromSource)); }

            T value = await fromSource().ConfigureAwait(false);
            await cacheProvider.SetAsync(cacheKey, value).ConfigureAwait(false);

            return value;
        } 
    }
}
