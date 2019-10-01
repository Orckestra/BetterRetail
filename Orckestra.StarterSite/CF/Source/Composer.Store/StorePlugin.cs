using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.Services;

namespace Orckestra.Composer.Store
{
    public class StorePlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {
            host.Register<GoogleMapsUrlProvider, IGoogleMapsUrlProvider>();
            host.Register<StoreViewService, IStoreViewService>();
            host.Register<StoreScheduleViewService, IStoreScheduleViewService>();
            host.Register<StoreLocatorViewService, IStoreLocatorViewService>();
            host.Register<StoreRepository, IStoreRepository>();
            host.Register<StoreViewModelFactory, IStoreViewModelFactory>();
            host.Register<StoreDirectoryViewService, IStoreDirectoryViewService>();
            host.Register<StoreInventoryViewService, IStoreInventoryViewService>();
            host.Register<StoreScheduleProvider, IStoreScheduleProvider>();
            host.Register<MapClustererProvider, IMapClustererProvider>();
            host.Register<MapConfigurationViewService, IMapConfigurationViewService>();
            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof(StorePlugin).Assembly);
            host.RegisterApiControllers(typeof(StorePlugin).Assembly);
        }
    }
}
