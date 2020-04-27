using System.Collections.Generic;
using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;
using System.Linq;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class ProductViewModel : BaseViewModel
    {
        [MapTo("Id")]
        public string ProductId { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        [Lookup(LookupType.Product, "Brand")]
        public string Brand { get; set; }

        [MapTo("Brand")]
        public string BrandId { get; set; }

        [MapTo("PrimaryParentCategoryId")]
        public string CategoryId { get; set; }

        //TODO move the the Context when available
        public string SelectedVariantId { get; set; }

        public string DisplayName { get; set; }

        [Formatting("General", "PriceFormat")]
        public string ListPrice { get; set; }

        public decimal? Price { get; set; }

        public string DefinitionName { get; set; }

        public string ProductDetailUrl { get; set; }

        public string FallbackImageUrl { get; set; }

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
        /// Is eligible if RecurringOrderProgramName is not null or empty and if recurring orders flag is enabled
        /// </summary>
        public bool IsRecurringOrderEligible { get; set; }
        /// <summary>
        /// Name of the recurring order program associated to the product
        /// </summary>
        public string RecurringOrderProgramName { get; set; }
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