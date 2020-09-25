using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.Website.ViewModels
{
    public interface IProductSearchResultsViewModelMetadata : IExtensionOf<ProductSearchResultsViewModel>
    {
        string ActionTarget { get; set; }
    }
}
