using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using Orckestra.Overture.ServiceModel.Requests.Products.Inventory;

namespace Orckestra.Composer.SearchQuery.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        public InventoryRepository(IOvertureClient overtureClient)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
        }

        protected IOvertureClient OvertureClient { get; set; }

        public async Task<List<InventoryItemAvailability>> GetInventoryLocationStatusesBySkus(GetInventoryLocationStatuseParam param)
        {
            var request = new FindInventoryItemsStatusByScopeAndSkusRequest
            {
                Skus = param.Skus,
                ScopeId = param.ScopeId,
                Date = DateTime.UtcNow
            };

            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);
            return param.InventoryLocationIds != null && param.InventoryLocationIds.Count > 0
                ? result.Where(d => d.Identifier != null && param.InventoryLocationIds.Contains(d.Identifier.InventoryLocationId)).ToList()
                : result;
        }
    }
}