using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeInventoryRepository: InventoryRepository
    {
        private readonly IScopeViewService _scopeService;
        public SalesScopeInventoryRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeViewService scopeService) : base(overtureClient, cacheProvider)
        {
            _scopeService = scopeService ?? throw new ArgumentNullException(nameof(scopeService));
        }

        public override async Task<InventoryItemStatusDetailsQueryResult> GetInventoryItemsBySkuAsync(GetInventoryItemsBySkuParam param)
        {
            param.Scope = await _scopeService.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetInventoryItemsBySkuAsync(param);
        }
    }
}
