using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Website.ViewModels
{
    public interface IProductSearchResultsViewModelMetadata : IExtensionOf<ProductSearchResultsViewModel>
    {
        string ActionTarget { get; set; }
    }
}
