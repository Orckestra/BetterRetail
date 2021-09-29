using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.Services;
using Orckestra.Composer.Store.ViewModels;

namespace Orckestra.Composer.Grocery.Services
{
    public class SalesScopeStoreViewService : StoreViewService
    {
        private readonly IScopeRepository _scopeRepository;

        public SalesScopeStoreViewService(
            IStoreRepository storeRepository,
            IStoreViewModelFactory storeViewModelFactory,
            ILocalizationProvider localizationProvider,
            IStoreUrlProvider storeUrlProvider,
            IGoogleSettings googleSettings,
            IScopeRepository scopeRepository)
            : base(storeRepository, storeViewModelFactory, localizationProvider, storeUrlProvider, googleSettings)
        {
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }

        public override async Task<StoreViewModel> GetStoreViewModelAsync(GetStoreByNumberParam param)
        {
            var salesScopeParam = param.Clone();
            salesScopeParam.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetStoreViewModelAsync(salesScopeParam).ConfigureAwait(false);
        }

        public override async Task<List<StoreViewModel>> GetStoresForInStorePickupViewModelAsync(
            GetStoresForInStorePickupViewModelParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetStoresForInStorePickupViewModelAsync(param).ConfigureAwait(false);
        }

        public override async Task<List<StoreViewModel>> GetAllStoresViewModelAsync(GetStoresParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetAllStoresViewModelAsync(param).ConfigureAwait(false);
        }
    }
}