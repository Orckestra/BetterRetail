using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels.Metadata
{
    public interface IProductSearchViewModelMetadata : IExtensionOf<ProductSearchViewModel>
    {
        string SearchTerm { get; set; }

    }
}