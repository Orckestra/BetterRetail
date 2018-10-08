using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.ViewModels.Inventory;

namespace Orckestra.Composer.Product.Services
{
    public interface IInventoryViewService
    {
        /// <summary>
        /// Retrieve the detail about the status of Inventory Items represented by the specified InventoryLocationId and a list of skus for the specified date
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<List<InventoryItemAvailabilityViewModel>> FindInventoryItemStatus(FindInventoryItemStatusParam param);
    }
}