using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Store.Extentions;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Models;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Overture.ServiceModel.Products.Inventory;


namespace Orckestra.Composer.Store.Services
{
    public class StoreInventoryViewService : IStoreInventoryViewService
    {
        protected IStoreViewModelFactory StoreViewModelFactory { get; private set; }
        protected IStoreRepository StoreRepository { get; private set; }
        protected ILookupService LookupService { get; private set; }
        protected IProductSettingsViewService ProductSettingsViewService { get; private set; }
        protected IInventoryRepository InventoryRepository { get; private set; }

        public StoreInventoryViewService(
            IStoreViewModelFactory storeViewModelFactory,
            IStoreRepository storeRepository,
            ILookupService lookupService,
            IProductSettingsViewService productSettingsViewService,
            IInventoryRepository inventoryRepository)
        {
            StoreViewModelFactory = storeViewModelFactory;
            StoreRepository = storeRepository;
            LookupService = lookupService;
            ProductSettingsViewService = productSettingsViewService;
            InventoryRepository = inventoryRepository;

        }

        public virtual async Task<StoreInventoryViewModel> GetStoreInventoryViewModelAsync(
            GetStoreInventoryViewModelParam viewModelParam)
        {
            ValidateParam(viewModelParam);

            var overtureInventoryItems =
                await InventoryRepository.GetInventoryItemsBySkuAsync(new GetInventoryItemsBySkuParam
                {
                    Scope = viewModelParam.Scope,
                    Sku = viewModelParam.Sku,
                    Date = viewModelParam.Date,
                    IncludeChildScopes = true

                }).ConfigureAwait(false);

            if (overtureInventoryItems == null)
            {
                return null;
            }

            var model = new StoreInventoryViewModel {Stores = new List<StoreViewModel>()};

            var stores = await GetStores(viewModelParam, overtureInventoryItems);

            if (stores.Count == 0)
            {
                return null;
            }

            var index = 1;
            if (viewModelParam.SearchPoint != null)
            {
                stores = stores.OrderBy(s => s.CalculateDestination(viewModelParam.SearchPoint)).ToList();
            }

            var storeIndexes = stores.ToDictionary(d => d.Number, d => index++);

            var storesForCurrentPage =
                stores.Skip((viewModelParam.PageNumber - 1)*viewModelParam.PageSize)
                    .Take(viewModelParam.PageSize)
                    .ToList();

            var isInventoryEnabled = await IsInventoryEnabledAsync(viewModelParam);
            var statusDisplayNames = await GetInventoryStatusDisplayNamesAsync(viewModelParam, isInventoryEnabled);

            foreach (var store in storesForCurrentPage)
            {
                var vm = StoreViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
                {
                    Store = store,
                    CultureInfo = viewModelParam.CultureInfo,
                    BaseUrl = viewModelParam.BaseUrl,
                    SearchPoint = viewModelParam.SearchPoint
                });

                vm.SearchIndex = storeIndexes[store.Number];

                var inventory =
                    overtureInventoryItems.Results.FirstOrDefault(
                        inv => inv.FulfillmentLocationNumber == store.Number);
                if (inventory != null)
                {
                    vm.InventoryStatus = GetInventoryStatusViewModel(isInventoryEnabled, inventory, statusDisplayNames);
                }

                model.Stores.Add(vm);
            }

            model.NextPage =
                StoreViewModelFactory.BuildNextPage(new GetStorePageViewModelParam
                {
                    Total = stores.Count,
                    PageSize = viewModelParam.PageSize,
                    CurrentPageNumber = viewModelParam.PageNumber
                });

            return model;
        }

        protected virtual StoreInventoryStatusViewModel GetInventoryStatusViewModel(bool isInventoryEnabled, InventoryItemStatusDetails inventory, Dictionary<string, string> statusDisplayNames)
        {
            return new StoreInventoryStatusViewModel
            {
                Status =
                    isInventoryEnabled
                        ? GetInventoryStatus(inventory.CurrentStatus)
                        : InventoryStatusEnum.Unspecified,
                DisplayName =
                    isInventoryEnabled
                        ? statusDisplayNames.FirstOrDefault(x => x.Key == inventory.CurrentStatus.ToString())
                            .Value
                        : string.Empty
            };
        }

        protected async virtual Task<Dictionary<string, string>> GetInventoryStatusDisplayNamesAsync
            (GetStoreInventoryViewModelParam param, bool isEnabled)
        {
            return isEnabled
                ? await LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
                {
                    CultureInfo = param.CultureInfo,
                    LookupType = LookupType.Order,
                    LookupName = "LineItemStatus"
                })
                : null;
        }

        protected virtual async Task<List<Overture.ServiceModel.Customers.Stores.Store>> GetStores(GetStoreInventoryViewModelParam viewModelParam,
            InventoryItemStatusDetailsQueryResult overtureInventoryItems)
        {
            var getStoresTasks = overtureInventoryItems.Results.Select(inventoryItem =>
                StoreRepository.GetStoreByNumberAsync(new GetStoreParam
                {
                    Scope = viewModelParam.Scope,
                    CultureInfo = viewModelParam.CultureInfo,
                    StoreNumber = inventoryItem.FulfillmentLocationNumber,
                    IncludeSchedules = true,
                    IncludeAddresses = true
                }));

            var result = await Task.WhenAll(getStoresTasks).ConfigureAwait(false);

            return result.Where(s => s != null && s.IsActive).ToList();
        }

        protected virtual InventoryStatusEnum GetInventoryStatus(InventoryStatus inventoryStatus)
        {
            switch (inventoryStatus)
            {
                case InventoryStatus.BackOrder:
                    return InventoryStatusEnum.BackOrder;
                case InventoryStatus.InStock:
                    return InventoryStatusEnum.InStock;
                case InventoryStatus.OutOfStock:
                    return InventoryStatusEnum.OutOfStock;
                case InventoryStatus.PreOrder:
                    return InventoryStatusEnum.PreOrder;
                default:
                    return InventoryStatusEnum.Unspecified;
            }
        }

        protected virtual async Task<bool> IsInventoryEnabledAsync(GetStoreInventoryViewModelParam param)
        {
            var productSettingsViewModel =
                await ProductSettingsViewService.GetProductSettings(param.Scope, param.CultureInfo);

            return productSettingsViewModel.IsInventoryEnabled;
        }

        protected static void ValidateParam(GetStoreInventoryViewModelParam viewModelParam)
        {
            if (viewModelParam == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(viewModelParam.Scope)) { throw new ArgumentNullException("scope"); }
            if (viewModelParam.CultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }
            if (string.IsNullOrWhiteSpace(viewModelParam.BaseUrl)) { throw new ArgumentNullException("baseUrl"); }
            if (string.IsNullOrWhiteSpace(viewModelParam.Sku)) { throw new ArgumentNullException("sku"); }
        }

    }
}
