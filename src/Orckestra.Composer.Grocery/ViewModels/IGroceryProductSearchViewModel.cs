using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryProductSearchViewModel : IExtensionOf<ProductSearchViewModel>
    {
        /// <summary>
        /// Gets or sets the product Badges
        /// </summary>
        string[] ProductBadges { get; set; }
    }
}
