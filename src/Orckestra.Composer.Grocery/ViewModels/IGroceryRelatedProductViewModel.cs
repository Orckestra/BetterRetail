using System.Collections.Generic;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryRelatedProductViewModel : IExtensionOf<RelatedProductViewModel>
    {
        /// <summary>
        /// Gets or sets the product Badges
        /// </summary>
        string ProductBadges { get; set; }
        Dictionary<string, string> ProductBadgeValues { get; set; }
    }
}
