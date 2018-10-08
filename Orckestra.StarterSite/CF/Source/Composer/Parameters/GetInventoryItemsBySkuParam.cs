using System;

namespace Orckestra.Composer.Parameters
{
    public class GetInventoryItemsBySkuParam
    {
        public string Scope { get; set; }

        /// <summary>
        /// The date which will be used to compute the status. If is not set, DateTime.Now will be used.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Sku which will be associated to the InventoryLocationId to retrieve InventoryItem
        /// </summary>
        public string Sku { get; set; }
        /// <summary>
        /// Wether inventory locations from child scopes should also be included in the results
        /// </summary>
        public bool IncludeChildScopes { get; set; }
    }
}
