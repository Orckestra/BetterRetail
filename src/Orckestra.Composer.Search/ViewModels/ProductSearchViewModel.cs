using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Search.ViewModels
{
    public class ProductSearchViewModel : BaseProductViewModel
    {
        /// <summary>
        /// Gets or sets the brand
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the full display name.
        /// </summary>
        [MapTo("DisplayName")]
        public string FullDisplayName { get; set; }

        public double? Price { get; set; }

        public double? ListPrice { get; set; }

        /// <summary>
        /// Gets or sets the search term
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the product Badges
        /// </summary>
        public string[] ProductBadges { get; set; }

        public Dictionary<string, string> ProductBadgeValues { get; set; }
        
        /// <summary>
        /// Gets or sets the product Promotional Ribbon
        /// </summary>
        public string PromotionalRibbon { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon Background Color
        /// </summary>
        public string PromotionalRibbonBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon Text Color
        /// </summary>
        public string PromotionalRibbonTextColor { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner
        /// </summary>
        public string PromotionalBanner { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner Background Color
        /// </summary>
        public string PromotionalBannerBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner Text Color
        /// </summary>
        public string PromotionalBannerTextColor { get; set; }

    }
}
