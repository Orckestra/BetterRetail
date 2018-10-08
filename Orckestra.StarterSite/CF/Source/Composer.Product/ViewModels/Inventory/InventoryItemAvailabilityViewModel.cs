using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels.Inventory
{
    public sealed class InventoryItemAvailabilityViewModel : BaseViewModel
    {
        /// <summary>
        /// The inventory item identifier
        /// </summary>
        public InventoryItemIdentifierViewModel Identifier { get; set; }

        /// <summary>
        /// The statuses
        /// </summary>
        public IList<InventoryItemStatusViewModel> Statuses { get; set; }

        /// <summary>
        /// The date
        /// </summary>
        public DateTime Date { get; set; }

        public InventoryItemAvailabilityViewModel()
        {
            Statuses = new List<InventoryItemStatusViewModel>();
        }
    }
}
