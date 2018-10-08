using System;
using System.Globalization;
using Orckestra.Composer.Store.Models;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreInventoryViewModelParam
    {
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        /// <summary>
        /// The date which will be used to compute the status. If is not set, DateTime.Now will be used.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Sku which will be associated to the InventoryLocationId to retrieve InventoryItem
        /// </summary>
        public string Sku { get; set; }

        public string BaseUrl { get; set; }

        public Coordinate SearchPoint { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
