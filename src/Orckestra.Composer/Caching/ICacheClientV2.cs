using System;

namespace Orckestra.Composer.Caching
{
    public interface ICacheClientV2 : ICacheClient
    {
        T GetOrAdd<T>(string key, Func<T> factory, TimeSpan? slidingExpiration = null);
    }
}
