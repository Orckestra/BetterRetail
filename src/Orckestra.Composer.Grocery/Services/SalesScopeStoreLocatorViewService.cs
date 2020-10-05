using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.Repositories;
using Orckestra.Composer.Store.Services;
using Orckestra.Composer.Store.ViewModels;

namespace Orckestra.Composer.Grocery.Services
{
    public class SalesScopeStoreLocatorViewService : StoreLocatorViewService
    {
        private readonly IScopeViewService _scopeService;

        public SalesScopeStoreLocatorViewService(
            IStoreRepository storeRepository,
            IStoreViewModelFactory storeViewModelFactory,
            IStoreUrlProvider storeUrlProvider,
            IMapClustererProvider mapClustererProvider,
            IGoogleSettings googleSettings,
            IScopeViewService scopeService)
            : base(storeRepository, storeViewModelFactory, storeUrlProvider, mapClustererProvider, googleSettings)
        {
            _scopeService = scopeService ?? throw new ArgumentNullException(nameof(scopeService));
        }

        public override async Task<StoreLocatorViewModel> GetStoreLocatorViewModelAsync(GetStoreLocatorViewModelParam param)
        {
            var salesScopeParam = param.Clone();
            salesScopeParam.Scope = await _scopeService.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetStoreLocatorViewModelAsync(salesScopeParam).ConfigureAwait(false);
        }
    }
}