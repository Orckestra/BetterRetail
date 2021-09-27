using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.Providers;
using Orckestra.Overture.ServiceModel.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopePaymentRepository: PaymentRepository
    {
        private readonly IScopeViewService _scopeService;
        public SalesScopePaymentRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeViewService scopeService) : base(overtureClient, cacheProvider)
        {
            _scopeService = scopeService ?? throw new ArgumentNullException(nameof(scopeService));
        }

        public override async Task<IList<PaymentProvider>> GetPaymentProviders(string scopeId)
        {
            var salesScope = await _scopeService.GetSaleScopeAsync(scopeId).ConfigureAwait(false);
            return await base.GetPaymentProviders(salesScope);
        }

        public override async Task<IList<Provider>> GetProviders(string scopeId, ProviderType providerType)
        {
            var salesScope = await _scopeService.GetSaleScopeAsync(scopeId).ConfigureAwait(false);
            return await base.GetProviders(salesScope, providerType);
        }
    }
}
