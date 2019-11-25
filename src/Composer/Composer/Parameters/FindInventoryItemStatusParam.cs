using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class FindInventoryItemStatusParam
    {
        /// <summary>
        /// The date for which to retrieve InventoryItemStatus
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The current scope id
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Sku which will be associated to the InventoryLocationId to retrieve InventoryItemStatus
        /// </summary>
        public List<string> Skus { get; set; }

        /// <summary>
        /// The current CultureInfo
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Inventory location Id to make the request.
        /// </summary>
        public string InventoryLocationId { get; set; }

        public FindInventoryItemStatusParam()
        {
            Skus = new List<string>();
        }
    }
}
