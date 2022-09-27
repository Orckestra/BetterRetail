using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;
using System.Globalization;

namespace Orckestra.Composer.Factory
{
    public interface IProductPricesViewModelFactory
    {
        ProductPriceViewModel CreateProductPriceViewModel(CultureInfo cultureInfo, ProductPrice productPrice);
    }
}
