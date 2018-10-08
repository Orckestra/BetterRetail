using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels.Inventory
{
    public sealed class InventoryScheduleViewModel : BaseViewModel
    {
        /// <summary>
        /// The inventory location identifier
        /// </summary>
        public string InventoryLocationId { get; set; }

        /// <summary>
        /// The associated product sku
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// The date range of the schedule
        /// </summary>
        public DateRangeViewModel DateRange { get; set; }

        /// <summary>
        /// The status of product inventory
        /// </summary>
        public InventoryStatusEnum InventoryStatus { get; set; }
    }
}
