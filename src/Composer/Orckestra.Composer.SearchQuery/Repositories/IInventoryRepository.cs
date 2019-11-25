using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.SearchQuery.Repositories
{
    /// <summary>
    /// Temporary class while seachqueries does not support Inventories
    /// </summary>
    public interface IInventoryRepository
    {
        Task<List<InventoryItemAvailability>> GetInventoryLocationStatusesBySkus(GetInventoryLocationStatuseParam param);
    }
}
