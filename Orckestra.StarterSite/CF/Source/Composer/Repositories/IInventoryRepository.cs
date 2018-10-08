using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Products.Inventory;

namespace Orckestra.Composer.Repositories
{
    public interface IInventoryRepository
    {
        /// <summary>
        /// Retrieve a list of InventoryItem represented by inventory location id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<List<InventoryItemAvailability>> FindInventoryItemStatus(FindInventoryItemStatusParam param);
        /// <summary>
        /// Retrieve a list of InventoryItemStatusDetails represented by sku from all inventory location associated to the specific scope.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<InventoryItemStatusDetailsQueryResult> GetInventoryItemsBySkuAsync(GetInventoryItemsBySkuParam param);
    }
}