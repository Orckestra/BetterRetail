using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Store.Extentions;
using Orckestra.Composer.Store.Models;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.ViewModels;

namespace Orckestra.Composer.Store.Services
{
    public class MapConfigurationViewService : IMapConfigurationViewService
    {
        protected IStoreRepository StoreRepository { get; private set; }
        public MapConfigurationViewService (IStoreRepository storeRepository)
        {
            StoreRepository = storeRepository;
        }

        public virtual async Task<MapConfigurationViewModel> GetMapConfigurationViewModelAsync(
            GetMapConfigurationViewModelParam param)
        {

            var vm = new MapConfigurationViewModel();
            vm.ZoomLevel = GoogleMapsConfiguration.ZoomLevel;
            vm.MarkerPadding = GoogleMapsConfiguration.MarkerPadding;

            if (param.LoadStoresBounds)
            {
                var stores = await StoreRepository.GetStoresAsync(new GetStoresParam { Scope = param.Scope, IncludeExtraInfo = false });
                vm.Bounds = GetStoresBounds(stores.Results);
            }
            return vm;
        }

        protected static Bounds GetStoresBounds(IList<Overture.ServiceModel.Customers.Stores.Store> stores)
        {
            var mapBounds = new Bounds();

            foreach (var store in stores.Where(store => store.HasLocation()))
                mapBounds.Extend(store.GetLatitude(), store.GetLongitude());

            return mapBounds;
        }
    }
}
