using System;
using System.Collections.Generic;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Grocery.ViewModels
{
    public interface IGroceryLineItemDetailViewModel : IExtensionOf<LineItemDetailViewModel>
    {
        /// <summary>
        /// Gets or sets the product Badges
        /// </summary>
        [MapTo("Product.ProductBadges")]
        string ProductBadges { get; set; }

        [Lookup(LookupType.Product, "ProductBadges")]
        [MapTo("Product.ProductBadges")]
        string ProductBadgesLookup { get; set; }

        Dictionary<string, string> ProductBadgeValues { get; set; }

        [MapTo("Product.IsWeightedProduct")]
        bool IsWeightedProduct { get; set; }

        /// <summary>
        /// Gets or sets the product unit quantity
        /// </summary>
        [MapTo("Product.ProductUnitQuantity")]
        int ProductUnitQuantity { get; set; }

        /// <summary>
        /// Gets or sets the product unit measure
        /// </summary>
        [Lookup(LookupType.Product, "UnitOfMeasure")]
        [MapTo("Product.ProductUnitMeasure")]
        string ProductUnitMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product unit size
        /// </summary>
        [MapTo("Product.ProductUnitSize")]
        Decimal ProductUnitSize { get; set; }

        [MapTo("Product.SellingMethod")]
        string SellingMethod { get; set; }

        [MapTo("Product.UnitOfMeasure")]
        string UnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the product Format
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon
        /// </summary>
        [MapTo("Product.PromotionalRibbon")]
        string PromotionalRibbonKey { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon
        /// </summary>
        [Lookup(LookupType.Product, "PromotionalRibbon")]
        [MapTo("Product.PromotionalRibbon")]
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
        [MapTo("Product.PromotionalBanner")]
        string PromotionalBannerKey { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner
        /// </summary>
        [Lookup(LookupType.Product, "PromotionalBanner")]
        [MapTo("Product.PromotionalBanner")]
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
