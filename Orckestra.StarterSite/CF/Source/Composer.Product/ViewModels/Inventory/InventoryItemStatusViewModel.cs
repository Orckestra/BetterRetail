using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels.Inventory
{
    public sealed class InventoryItemStatusViewModel : BaseViewModel
    {
        /// <summary>
        /// The inventory status
        /// </summary>
        public InventoryStatusEnum Status { get; set; }

        /// <summary>
        /// The quantified information of this item
        /// </summary>
        public double? Quantity { get; set; }
    }
}
