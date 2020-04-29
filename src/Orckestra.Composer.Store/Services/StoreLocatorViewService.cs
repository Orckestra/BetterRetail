using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Store.Extentions;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Overture.ServiceModel.Customers;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Store.Services
{
    public class StoreLocatorViewService : IStoreLocatorViewService
    {
        protected IStoreRepository StoreRepository { get; private set; }
        protected IStoreViewModelFactory StoreViewModelFactory { get; private set; }
        protected IStoreUrlProvider StoreUrlProvider { get; private set; }
        protected IMapClustererProvider MapClustererProvider { get; private set; }

        public StoreLocatorViewService(
            IStoreRepository storeRepository,
            IStoreViewModelFactory storeViewModelFactory,
            IStoreUrlProvider storeUrlProvider,
            IMapClustererProvider mapClustererProvider)
        {
            StoreRepository = storeRepository;
            StoreViewModelFactory = storeViewModelFactory;
            StoreUrlProvider = storeUrlProvider;
            MapClustererProvider = mapClustererProvider;
        }
        public virtual async Task<StoreLocatorViewModel> GetStoreLocatorViewModelAsync(GetStoreLocatorViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var overtureStores = await StoreRepository.GetStoresAsync(new GetStoresParam 
            { 
                Scope = param.Scope, 
                IncludeExtraInfo = true 
            }).ConfigureAwait(false);

            var model = GetEmptyStoreLocatorViewModel(new GetEmptyStoreLocatorViewModelParam
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo
            });

            IEnumerable<Overture.ServiceModel.Customers.Stores.Store> stores = overtureStores.Results;

            var index = 1;

            if (param.MapBounds != null)
            {
                stores = stores.Where(st => st.InBounds(param.MapBounds));
            }

            if (param.SearchPoint != null)
            {
                stores = stores.OrderBy(s => s.CalculateDestination(param.SearchPoint));
                model.NearestStoreCoordinate = new StoreGeoCoordinate(stores.FirstOrDefault());
            }
            var storeIndexes = stores.ToDictionary(d => d.Number, d => index++);

            var storesForCurrentPage = stores.Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize).ToList();

            var schedules = await GetStoreSchedules(storesForCurrentPage, param);

            foreach (var store in storesForCurrentPage)
            {
                if (store.StoreSchedule == null)
                {
                    store.StoreSchedule = schedules.FirstOrDefault(
                        s => s.FulfillmentLocationId == store.FulfillmentLocation.Id.ToString());
                }

                var vm = StoreViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
                {
                    Store = store,
                    CultureInfo = param.CultureInfo,
                    BaseUrl = param.BaseUrl,
                    SearchPoint = param.SearchPoint
                });

                vm.SearchIndex = storeIndexes[store.Number];

                model.Stores.Add(vm);
            }

            if (param.IncludeMarkers)
            {
                model.Markers = GetMarkers(param, stores.ToList(), storeIndexes);
            }

            model.NextPage = StoreViewModelFactory.BuildNextPage(new GetStorePageViewModelParam
            {
                Total = stores.Count(),
                PageSize = param.PageSize,
                CurrentPageNumber = param.PageNumber
            });

            return model;
        }

        protected virtual List<StoreClusterViewModel> GetMarkers(
            GetStoreLocatorViewModelParam param,
            List<Overture.ServiceModel.Customers.Stores.Store> stores, 
            Dictionary<string, int> storeIndexes)
        {
            var getClusterParams = new GetMapClustersParam
            {
                ZoomLevel = param.ZoomLevel,
                SearchPoint = param.SearchPoint,
                Locations = stores.Select(s => new StoreGeoCoordinate(s)).Cast<IGeoCoordinate>().ToList()
            };
            var clusters = MapClustererProvider.GetMapClusters(getClusterParams);

            return clusters.Select(c =>
                new StoreClusterViewModel
                {
                    Center = c.Center,
                    Tile = c.Tile,
                    ItemsCount = c.ItemsCount,
                    StoreNumber = c.StoreNumber,
                    SearchIndex = c.ItemsCount == 1 ? storeIndexes[c.StoreNumber] : 0
                }).ToList();
        }
        protected virtual async Task<FulfillmentSchedule[]> GetStoreSchedules(
            List<Overture.ServiceModel.Customers.Stores.Store> stores, GetStoreLocatorViewModelParam param)
        {
            var getSchedulesTasks = stores.Where(s => s.StoreSchedule == null).Select(s =>
                StoreRepository.GetStoreScheduleAsync(new GetStoreScheduleParam
                {
                    Scope = param.Scope,
                    CultureInfo = param.CultureInfo,
                    FulfillmentLocationId = s.FulfillmentLocation.Id
                }));

            return await Task.WhenAll(getSchedulesTasks).ConfigureAwait(false);
        }

        public virtual StoreLocatorViewModel GetEmptyStoreLocatorViewModel(GetEmptyStoreLocatorViewModelParam viewModelParam)
        {
            if (string.IsNullOrWhiteSpace(viewModelParam.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(viewModelParam.BaseUrl)), nameof(viewModelParam)); }
            if (viewModelParam.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(viewModelParam.CultureInfo)), nameof(viewModelParam)); }

            var model = new StoreLocatorViewModel
            {
                Stores = new List<StoreViewModel>(),
                StoresDirectoryUrl = StoreUrlProvider.GetStoresDirectoryUrl(new GetStoresDirectoryUrlParam
                {
                    CultureInfo = viewModelParam.CultureInfo,
                    BaseUrl = viewModelParam.BaseUrl,
                    Page = 1
                })
            };
            return model;
        }
    }
}