namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    public interface ICacheService
    {
        ICacheStore<K, V> GetStore<K, V>(string name, int maximumSize);
        ICacheStore<K, V> GetStore<K, V>(string name);
        ICacheStore<K, V> GetStoreWithDependencies<K, V>(string name, int maximumSize, params CacheDependentEntry[] dependentEntities);
        ICacheStore<K, V> GetStoreWithDependencies<K, V>(string name, params CacheDependentEntry[] dependentEntities);
    };
}