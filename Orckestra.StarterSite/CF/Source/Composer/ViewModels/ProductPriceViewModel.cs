using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ViewModels
{
    /// <summary>
    /// Product Price View Model Item
    /// </summary>
    public sealed class ProductPriceViewModel : BaseViewModel
    {
        public string ProductId { get; set; }

        public bool IsPriceDiscounted { get; set; }

        [MapTo("DefaultPrice")]
        [Formatting("General", "PriceFormat")]
        public string DefaultListPrice { get; set; }

        [Formatting("General", "PriceFormat")]
        public string ListPrice { get; set; }

        public List<VariantPriceViewModel> VariantPrices { get; set; }

        public ProductPriceViewModel()
        {
            VariantPrices = new List<VariantPriceViewModel>();
        }
    }
}
