using Orckestra.Composer.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class ProductViewModel : BaseProductViewModel
    {
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

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is on sale.
        /// </summary>
        /// <remarks>
        ///     If true Price/DisplaySpecialPrice will contains the discount price and ListPrice/DisplayPrice will contains the regular price
        ///     If false Price/DisplaySpecialPrice will be set to null and List/DisplayPrice will contains the regular Price
        /// </remarks>
        public bool IsOnSale { get { return Price < ListPrice; } }

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

        public ProductViewModel()
        {
            KeyVariantAttributeItems = new List<KeyVariantAttributeItem>();
            Images = new List<ProductDetailImageViewModel>();
            LocalizedDisplayNames = new Dictionary<string, string>();
            RecurringOrderFrequencies = new List<RecurringOrderProgramFrequencyViewModel>();
        }
    }
}