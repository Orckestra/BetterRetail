using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeFulfillmentMethodRepository : FulfillmentMethodRepository
    {

        private readonly IScopeRepository _scopeRepository;
        public SalesScopeFulfillmentMethodRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeRepository scopeRepository) : base(overtureClient, cacheProvider)
        {
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }
        public override async Task<List<FulfillmentMethod>> GetCalculatedFulfillmentMethods(GetShippingMethodsParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetCalculatedFulfillmentMethods(param);
        }

        public override async Task<GetFulfillmentMethodsResponse> GetFulfillmentMethods(string scopeId)
        {
            var saleScopeId = await _scopeRepository.GetSaleScopeAsync(scopeId).ConfigureAwait(false);
            return await base.GetFulfillmentMethods(saleScopeId);
        }
    }
}
