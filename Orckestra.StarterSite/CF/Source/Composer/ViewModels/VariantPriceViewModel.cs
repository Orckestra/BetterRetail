using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Variant Product Price View Model Item.  (This object is inside a list of the Product Price View Model Item.
    /// </summary>
    public sealed class VariantPriceViewModel : BaseViewModel
    {
        public string VariantId { get; set; }

        public bool IsPriceDiscounted { get; set; }

        [MapTo("DefaultPrice")]
        [Formatting("General", "PriceFormat")]
        public string DefaultListPrice { get; set; }

        [Formatting("General", "PriceFormat")]
        public string ListPrice { get; set; }
    }
}
