using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Providers
{
    public interface IInventoryLocationProvider
    {
        /// <summary>
        /// Business id for the default inventory location which will be associated to the Sku to retrieve InventoryItemStatus
        /// </summary>
        /// <returns></returns>
        Task<string> GetDefaultInventoryLocationIdAsync();

        /// <summary>
        /// Business id for the inventory locations which will be used by product searches
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetInventoryLocationIdsForSearchAsync();

        /// <summary>
        /// Obtains the fulfillment location to use for a cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<FulfillmentLocation> GetFulfillmentLocationAsync(GetFulfillmentLocationParam param);

        /// <summary>
        /// Set InventoryLocationId (use for test only)
        /// </summary>
        /// <param name="inventoryLocationId"></param>
        /// <returns></returns>
        string SetDefaultInventoryLocationId(string inventoryLocationId);
    }
}