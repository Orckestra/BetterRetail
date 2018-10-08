using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
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
