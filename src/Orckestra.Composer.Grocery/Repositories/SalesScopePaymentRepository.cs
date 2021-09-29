using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.Providers;
using Orckestra.Overture.ServiceModel.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopePaymentRepository: PaymentRepository
    {
        private readonly IScopeRepository _scopeRepository;
        public SalesScopePaymentRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeRepository scopeRepository) : base(overtureClient, cacheProvider)
        {
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }

        public override async Task<IList<PaymentProvider>> GetPaymentProviders(string scopeId)
        {
            var salesScope = await _scopeRepository.GetSaleScopeAsync(scopeId).ConfigureAwait(false);
            return await base.GetPaymentProviders(salesScope);
        }

        public override async Task<IList<Provider>> GetProviders(string scopeId, ProviderType providerType)
        {
            var salesScope = await _scopeRepository.GetSaleScopeAsync(scopeId).ConfigureAwait(false);
            return await base.GetProviders(salesScope, providerType);
        }
    }
}
