using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels.Inventory
{
    public sealed class InventoryItemIdentifierViewModel : BaseViewModel
    {
        /// <summary>
        /// The InventoryLocation identifier
        /// </summary>
        public string InventoryLocationId { get; set; }

        /// <summary>
        /// The Sku (product identifier)
        /// </summary>
        public string Sku { get; set; }
    }
}
