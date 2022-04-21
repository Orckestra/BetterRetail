using System;
using System.Collections.Generic;
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
        Dictionary<string, string> ProductBadgeValues { get; set; }

        /// <summary>
        /// Gets or sets the product Measure
        /// </summary>
        String BaseProductMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product Size
        /// </summary>
        Single BaseProductSize { get; set; }

        /// <summary>
        /// Gets or sets the product unit quantity
        /// </summary>
        int ProductUnitQuantity { get; set; }

        /// <summary>
        /// Gets or sets the product unit measure
        /// </summary>
        String ProductUnitMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product unit size
        /// </summary>
        Single ProductUnitSize { get; set; }

        /// <summary>
        /// Gets or sets the product Converted Volume Measurements
        /// </summary>
        decimal ConvertedVolumeMeasurement { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon
        /// </summary>
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
        string PromotionalBanner { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner Background Color
        /// </summary>
        string PromotionalBannerBackgroundColor { get; set; }

        // <summary>
        /// Gets or sets the product Promotional Banner Text Color
        /// </summary>
        string PromotionalBannerTextColor { get; set; }

        /// <summary>
        /// Gets or sets the product Format
        /// </summary>
        string Format { get; set; }
    }
}
