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

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeFulfillmentMethodRepository : FulfillmentMethodRepository
    {

        private readonly IScopeViewService _scopeService;
        public SalesScopeFulfillmentMethodRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeViewService scopeService) : base(overtureClient, cacheProvider)
        {
            _scopeService = scopeService ?? throw new ArgumentNullException(nameof(scopeService));
        }
        public override async Task<List<FulfillmentMethod>> GetCalculatedFulfillmentMethods(GetShippingMethodsParam param)
        {
            param.Scope = await _scopeService.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetCalculatedFulfillmentMethods(param);
        }

        public override async Task<GetFulfillmentMethodsResponse> GetFulfillmentMethods(string scopeId)
        {
            var saleScopeId = await _scopeService.GetSaleScopeAsync(scopeId).ConfigureAwait(false);
            return await base.GetFulfillmentMethods(saleScopeId);
        }
    }
}
