using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
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
    public class SalesScopeStoreLocatorViewService : StoreLocatorViewService
    {
        private readonly IScopeRepository _scopeRepository;

        public SalesScopeStoreLocatorViewService(
            IStoreRepository storeRepository,
            IStoreViewModelFactory storeViewModelFactory,
            IStoreUrlProvider storeUrlProvider,
            IMapClustererProvider mapClustererProvider,
            IGoogleSettings googleSettings,
            IScopeRepository scopeRepository)
            : base(storeRepository, storeViewModelFactory, storeUrlProvider, mapClustererProvider, googleSettings)
        {
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }

        public override async Task<StoreLocatorViewModel> GetStoreLocatorViewModelAsync(GetStoreLocatorViewModelParam param)
        {
            var salesScopeParam = param.Clone();
            salesScopeParam.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetStoreLocatorViewModelAsync(salesScopeParam).ConfigureAwait(false);
        }
    }
}