using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class RelatedProductViewModel : BaseProductViewModel
    {
        ///The output from OCC contains lookup value, to get display name 
        ///is used <see cref="Lookup{TKey, TElement}"></see> attribute
        [Lookup(LookupType.Product, "Brand")]
        public string Brand { get; set; }

        //Must be parsed from Overture's output.
        [MapTo("Brand")]
        public string BrandId { get; set; }

        /// <summary>
        /// The base price for the product
        /// </summary>
        public decimal? ListPrice { get; set; }
        /// <summary>
        /// The current price of the product. This will usually be the same as <see cref="ListPrice"/>,
        /// but could be less if the product is discounted.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        public ProductQuantityViewModel Quantity { get; set; }

        /// <summary>
        /// Gets or sets the product Badges
        /// </summary>
        [MapTo("ProductBadges")]
        public string ProductBadgesKeys { get; set; }

        [Lookup(LookupType.Product, "ProductBadges")]
        [MapTo("ProductBadges")]
        public string ProductBadgesLookup { get; set; }

        public Dictionary<string, string> ProductBadgeValues { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon Value
        /// </summary>
        [MapTo("PromotionalRibbon")]
        public string PromotionalRibbonValue { get; set; }
        /// <summary>
        /// Gets or sets the product Promotional Ribbon
        /// </summary>
        [Lookup(LookupType.Product, "PromotionalRibbon")]
        public string PromotionalRibbon { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Ribbon Background Color
        /// </summary>
        public string PromotionalRibbonBackgroundColor { get; set; }

        // <summary>
        /// Gets or sets the product Promotional Ribbon Text Color
        /// </summary>
        public string PromotionalRibbonTextColor { get; set; }


        /// <summary>
        /// Gets or sets the product Promotional Banner Value
        /// </summary>
        [MapTo("PromotionalBanner")]
        public string PromotionalBannerValue { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner
        /// </summary>
        [Lookup(LookupType.Product, "PromotionalBanner")]
        public string PromotionalBanner { get; set; }

        /// <summary>
        /// Gets or sets the product Promotional Banner Background Color
        /// </summary>
        public string PromotionalBannerBackgroundColor { get; set; }

        // <summary>
        /// Gets or sets the product Promotional Banner Text Color
        /// </summary>
        public string PromotionalBannerTextColor { get; set; }

    }
}
