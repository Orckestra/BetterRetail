using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public class ProductSearchViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product variant identifier.
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        /// True if the products has variants
        /// </summary>
        public bool HasVariants { get; set; }

        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the display name. May be truncated.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the full display name.
        /// </summary>
        [MapTo("DisplayName")]
        public string FullDisplayName { get; set; }

        //Not a lookup in this case.
        public string Brand { get; set; }

        //Must be parsed from Overture's output.
        public string BrandId { get; set; }

        /// <summary>
        /// Gets or sets the Definition Name
        /// </summary>
        public string DefinitionName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the fallback image URL.
        /// </summary>
        public string FallbackImageUrl { get; set; }

        /// <summary>
        /// Gets or sets product prices.
        /// </summary>
        public ProductPriceSearchViewModel Pricing { get; set; }

        /// <summary>
        /// If the Product is available for sell
        /// </summary>
        public bool IsAvailableToSell { get; set; }

        /// <summary>
        /// Id of the primary category.
        /// </summary>
        [MapTo("PrimaryParentCategoryId")]
        public string CategoryId { get; set; }

        /// <summary>
        /// If the product has a program and the flag is enabled
        /// </summary>
        public bool? IsEligibleForRecurring { get; set; }

        /// <summary>
        /// Gets or sets the recurring program name
        /// </summary>
        public string RecurringOrderProgramName { get; set; }

        /// <summary>
        /// Gets or sets the search term
        /// </summary>
        public string SearchTerm { get; set; }
    }
}
