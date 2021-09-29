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
        private readonly IScopeRepository _scopeRepository;
        public SalesScopeInventoryRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider, IScopeRepository scopeRepository) : base(overtureClient, cacheProvider)
        {
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }

        public override async Task<InventoryItemStatusDetailsQueryResult> GetInventoryItemsBySkuAsync(GetInventoryItemsBySkuParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetInventoryItemsBySkuAsync(param);
        }
    }
}
