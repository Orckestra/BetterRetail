namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    public interface ICacheService
    {
        ICacheStore<K, V> GetStore<K, V>(string name, int maximumSize) where V: class;
        ICacheStore<K, V> GetStore<K, V>(string name) where V: class;
        ICacheStore<K, V> GetStoreWithDependencies<K, V>(string name, int maximumSize, params CacheDependentEntry[] dependentEntities) where V : class;
        ICacheStore<K, V> GetStoreWithDependencies<K, V>(string name, params CacheDependentEntry[] dependentEntities) where V : class;
    };
}