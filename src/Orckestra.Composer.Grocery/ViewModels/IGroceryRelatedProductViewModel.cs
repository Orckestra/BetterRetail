using System.Collections.Generic;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryRelatedProductViewModel : IExtensionOf<RelatedProductViewModel>
    {
        /// <summary>
        /// Gets or sets the product Measure
        /// </summary>
        [Lookup(LookupType.Product, "UnitOfMeasure")]
        string BaseProductMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product Size
        /// </summary>
        decimal BaseProductSize { get; set; }

        /// <summary>
        /// Gets or sets the product unit quantity
        /// </summary>
        int ProductUnitQuantity { get; set; }

        /// <summary>
        /// Gets or sets the product unit measure
        /// </summary>
        [Lookup(LookupType.Product, "UnitOfMeasure")]
        string ProductUnitMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product unit size
        /// </summary>
        decimal ProductUnitSize { get; set; }

        /// <summary>
        /// Gets or sets the product Converted Volume Measurements
        /// </summary>
        decimal ConvertedVolumeMeasurement { get; set; }

        /// <summary>
        /// Gets or sets the product IsWeightedProduct
        /// </summary>
        bool IsWeightedProduct { get; set; }

        /// <summary>
        /// Gets or sets the product Badges
        /// </summary>
        string ProductBadges { get; set; }
        Dictionary<string, string> ProductBadgeValues { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon
        /// </summary>
        [Lookup(LookupType.Product, "PromotionalRibbon")]
        string PromotionalRibbon { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon Background Color
        /// </summary>
        string PromotionalRibbonBackgroundColor { get; set; }

        // <summary>
        /// Gets or sets the product Promotional Ribbon Text Color
        /// </summary>
        string PromotionalRibbonTextColor { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner
        /// </summary>
        [Lookup(LookupType.Product, "PromotionalBanner")]
        string PromotionalBanner { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner Background Color
        /// </summary>
        string PromotionalBannerBackgroundColor { get; set; }

        // <summary>
        /// Gets or sets the product Promotional Banner Text Color
        /// </summary>
        string PromotionalBannerTextColor { get; set; }
    }
}
