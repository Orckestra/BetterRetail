using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.ViewModels
{
    public sealed class ProductsPricesViewModel : BaseViewModel
    {
        public List<ProductPriceViewModel> ProductPrices { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public ProductsPricesViewModel()
        {
            ProductPrices = new List<ProductPriceViewModel>();
        }
    }
}
