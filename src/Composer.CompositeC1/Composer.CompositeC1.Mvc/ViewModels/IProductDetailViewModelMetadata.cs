using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.CompositeC1.Mvc.ViewModels
{
    public interface IProductDetailViewModelMetadata : IExtensionOf<ProductViewModel>
    {

        string Manufacturer { get; set; }
    }

}