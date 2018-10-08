using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Parameters
{
    public class CreateProductPriceViewModelParam
    {
        public CultureInfo CultureInfo { get; set; }
        public IList<ProductPrice> ProductPrices { get; set; }

        public CurrencyViewModel CurrencyViewModel { get; set; }

        public CreateProductPriceViewModelParam()
        {
            ProductPrices = new List<ProductPrice>();
        }
    }
}