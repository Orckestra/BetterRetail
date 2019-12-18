using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public sealed class InventoryItemStatusViewModel : BaseViewModel
    {
        /// <summary>
        /// The quantified information of this item
        /// </summary>
        public InventoryStatusEnum Status { get; set; }

        /// <summary>
        /// The quantity
        /// </summary>
        public double? Quantity { get; set; }
    }
}
