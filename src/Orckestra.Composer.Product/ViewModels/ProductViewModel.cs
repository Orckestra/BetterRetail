using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class ProductViewModel : BaseProductViewModel
    {
        ///The output from OCC contains lookup value, to get display name 
        ///is used <see cref="Lookup{TKey, TElement}"></see> attribute
        [Lookup(LookupType.Product, "Brand")]
        public string Brand { get; set; }

        //Must be parsed from Overture's output.
        [MapTo("Brand")]
        public string BrandId { get; set; }

        public string SelectedVariantId { get; set; }

        /// <summary>
        /// The base price for the product
        /// </summary>
        public decimal? ListPrice { get; set; }
        /// <summary>
        /// The current price of the product. This will usually be the same as <see cref="ListPrice"/>,
        /// but could be less if the product is discounted.
        /// </summary>
        public decimal? Price { get; set; }

        public CurrencyViewModel Currency { get; set; }

        public ProductDetailImageViewModel SelectedImage { get; set; }

        public List<KeyVariantAttributeItem> KeyVariantAttributeItems { get; set; }

        public List<VariantViewModel> Variants { get; set; }

        public List<ProductDetailImageViewModel> Images { get; set; }

        public bool IsProductZoomEnabled
        {
            get { return Images.All(image => image.IsProductZoomImageUrlDefined); }
        }

        public ProductQuantityViewModel Quantity { get; set; }

        //todo: remove it after language switch is refactored, use viewmodel that will be implemented
        [MapTo("DisplayName")]
        public Dictionary<string, string> LocalizedDisplayNames { get; set; }

        /// <summary>
        /// List of frequencies available based on the program
        /// </summary>
        public List<RecurringOrderProgramFrequencyViewModel> RecurringOrderFrequencies { get; set; }

        public string CreateAccountUrl { get; set; }

        public SpecificationsViewModel Specifications { get; set; }

        public decimal? Weight { get; set; }

        public string WeightUOM { get; set; }

        public decimal ItemFormat { get; set; }

        /// <summary>
        /// Gets or sets the product Badges
        /// </summary>
        [MapTo("ProductBadges")]
        public string ProductBadgesKeys { get; set; }

        [Lookup(LookupType.Product, "ProductBadges")]
        [MapTo("ProductBadges")]
        public string ProductBadgesLookup { get; set; }

        public Dictionary<string, string> ProductBadgeValues { get; set; }

        public ProductViewModel()
        {
            KeyVariantAttributeItems = new List<KeyVariantAttributeItem>();
            Images = new List<ProductDetailImageViewModel>();
            LocalizedDisplayNames = new Dictionary<string, string>();
            RecurringOrderFrequencies = new List<RecurringOrderProgramFrequencyViewModel>();
        }
    }
}