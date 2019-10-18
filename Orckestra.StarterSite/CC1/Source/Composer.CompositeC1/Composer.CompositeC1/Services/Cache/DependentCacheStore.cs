using System.Collections.Generic;
using System.Linq;
using Composite.Data;

namespace Orckestra.Composer.CompositeC1.Services.Cache
{
    internal class DependentCacheStore<K, V> : CacheStore<K, V> where V : class
    {
        public List<CacheDependentEntry> DependentEntities { get; }
        public DependentCacheStore(string name, int maximumSize, params CacheDependentEntry[] dependentEntities) : base(name, maximumSize)
        {
            DependentEntities = dependentEntities.ToList();
            SubscribeToEvents(dependentEntities);
        }

        public DependentCacheStore(string name, params CacheDependentEntry[] dependentEntities) : base(name)
        {
            DependentEntities = dependentEntities.ToList();
            SubscribeToEvents(dependentEntities);
        }

        private void SubscribeToEvents(IEnumerable<CacheDependentEntry> dependentEntities)
        {
            foreach (var dependentEntry in dependentEntities)
            {
                if (dependentEntry.Operations.HasFlag(CacheDependentOperations.Add))
                    DataEventSystemFacade.SubscribeToDataAfterAdd(dependentEntry.EntityType, ResetCache, true);

                if (dependentEntry.Operations.HasFlag(CacheDependentOperations.Update))
                    DataEventSystemFacade.SubscribeToDataAfterUpdate(dependentEntry.EntityType, ResetCache, true);
                
                if (dependentEntry.Operations.HasFlag(CacheDependentOperations.Deleted))
                    DataEventSystemFacade.SubscribeToDataDeleted(dependentEntry.EntityType, ResetCache, true);
            }
        }

        private void UnsubscribeFromEvents(IEnumerable<CacheDependentEntry> dependentEntities)
        {
            foreach (var dependentEntry in dependentEntities)
            {
                if (dependentEntry.Operations.HasFlag(CacheDependentOperations.Add))
                    DataEventSystemFacade.UnsubscribeToDataAfterAdd(dependentEntry.EntityType, ResetCache);

                if (dependentEntry.Operations.HasFlag(CacheDependentOperations.Update))
                    DataEventSystemFacade.UnsubscribeToDataAfterUpdate(dependentEntry.EntityType, ResetCache);
                
                if (dependentEntry.Operations.HasFlag(CacheDependentOperations.Deleted))
                    DataEventSystemFacade.UnsubscribeToDataDeleted(dependentEntry.EntityType, ResetCache);
            }
        }

        private void ResetCache(object sender, DataEventArgs dataEventArgs)
        {
            Clear();
        }
        
        public override void Dispose()
        {
            UnsubscribeFromEvents(DependentEntities);
            base.Dispose();
        }
    };
}