using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.Website.ViewModels
{
    public interface IProductDetailViewModelMetadata : IExtensionOf<ProductViewModel>
    {

        string Manufacturer { get; set; }
    }

}