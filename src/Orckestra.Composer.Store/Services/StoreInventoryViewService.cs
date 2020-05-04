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
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

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

        public virtual async Task<StoreInventoryViewModel> GetStoreInventoryViewModelAsync(GetStoreInventoryViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Sku)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Sku)), nameof(param)); }

            var overtureInventoryItems = await InventoryRepository.GetInventoryItemsBySkuAsync(new GetInventoryItemsBySkuParam
            {
                Scope = param.Scope,
                Sku = param.Sku,
                Date = param.Date,
                IncludeChildScopes = true
                }).ConfigureAwait(false);

            if (overtureInventoryItems == null) { return null; }

            var model = new StoreInventoryViewModel { Stores = new List<StoreViewModel>() };

            var stores = await GetStores(param, overtureInventoryItems);

            if (stores.Count == 0) { return null; }

            var index = 1;
            if (param.SearchPoint != null)
            {
                stores = stores.OrderBy(s => s.CalculateDestination(param.SearchPoint)).ToList();
            }

            var storeIndexes = stores.ToDictionary(d => d.Number, d => index++);

            var storesForCurrentPage =
                stores.Skip((param.PageNumber - 1)*param.PageSize)
                    .Take(param.PageSize)
                    .ToList();

            var isInventoryEnabled = await IsInventoryEnabledAsync(param);
            var statusDisplayNames = await GetInventoryStatusDisplayNamesAsync(param, isInventoryEnabled);

            foreach (var store in storesForCurrentPage)
            {
                var vm = StoreViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
                {
                    Store = store,
                    CultureInfo = param.CultureInfo,
                    BaseUrl = param.BaseUrl,
                    SearchPoint = param.SearchPoint
                });

                vm.SearchIndex = storeIndexes[store.Number];

                var inventory = overtureInventoryItems.Results.FirstOrDefault(inv => inv.FulfillmentLocationNumber == store.Number);
                if (inventory != null)
                {
                    vm.InventoryStatus = GetInventoryStatusViewModel(isInventoryEnabled, inventory, statusDisplayNames);
                }

                model.Stores.Add(vm);
            }

            model.NextPage = StoreViewModelFactory.BuildNextPage(new GetStorePageViewModelParam
            {
                Total = stores.Count,
                PageSize = param.PageSize,
                CurrentPageNumber = param.PageNumber
            });

            return model;
        }

        protected virtual StoreInventoryStatusViewModel GetInventoryStatusViewModel(
            bool isInventoryEnabled, 
            InventoryItemStatusDetails inventory, 
            Dictionary<string, string> statusDisplayNames)
        {
            return new StoreInventoryStatusViewModel
            {
                Status = isInventoryEnabled
                        ? GetInventoryStatus(inventory.CurrentStatus)
                        : InventoryStatusEnum.Unspecified,
                DisplayName = isInventoryEnabled
                        ? statusDisplayNames.FirstOrDefault(x => x.Key == inventory.CurrentStatus.ToString()).Value
                        : string.Empty
            };
        }

        protected async virtual Task<Dictionary<string, string>> GetInventoryStatusDisplayNamesAsync(GetStoreInventoryViewModelParam param, bool isEnabled)
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

        protected virtual async Task<List<Overture.ServiceModel.Customers.Stores.Store>> GetStores(
            GetStoreInventoryViewModelParam viewModelParam,
            InventoryItemStatusDetailsQueryResult overtureInventoryItems)
        {
            var getStoresTasks = overtureInventoryItems.Results.Select(inventoryItem =>
                StoreRepository.GetStoreByNumberAsync(new GetStoreByNumberParam
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
            var productSettingsViewModel = await ProductSettingsViewService.GetProductSettings(param.Scope, param.CultureInfo);

            return productSettingsViewModel.IsInventoryEnabled;
        }
    }
}