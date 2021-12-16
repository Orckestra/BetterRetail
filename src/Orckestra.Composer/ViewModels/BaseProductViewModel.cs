
namespace Orckestra.Composer.ViewModels
{
    public class BaseProductViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        [MapTo("Id")]
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
        /// Gets or sets the Definition Name
        /// </summary>
        public string DefinitionName { get; set; }

        /// <summary>
        /// Gets or sets the display name. May be truncated.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL to the Product Details Page.
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
        /// If the Product is available for sell
        /// </summary>
        public bool IsAvailableToSell { get; set; }

        /// <summary>
        /// Id of the primary category.
        /// </summary>
        [MapTo("PrimaryParentCategoryId")]
        public string CategoryId { get; set; }

        /// <summary>
        /// Is eligible if RecurringOrderProgramName is not null or empty and if recurring orders flag is enabled
        /// </summary>
        public bool IsRecurringOrderEligible { get; set; }

        /// <summary>
        /// Gets or sets the recurring program name
        /// </summary>
        public string RecurringOrderProgramName { get; set; }

        /// <summary>
        /// Gets or sets the display base price of teh product.
        /// </summary>
        [Formatting("General", "PriceFormat")]
        [MapTo("ListPrice")]
        public string DisplayListPrice { get; set; }

        /// <summary>
        ///     Gets or sets the display special price.
        /// </summary>
        public string DisplaySpecialPrice { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is on sale.
        /// </summary>
        /// <remarks>
        ///     If true Price/DisplaySpecialPrice will contains the discount price and ListPrice/DisplayPrice will contains the regular price
        ///     If false Price/DisplaySpecialPrice will be set to null and List/DisplayPrice will contains the regular Price
        /// </remarks>
        public bool IsOnSale { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating wether this instance has a range of price.
        /// </summary>
        public bool HasPriceRange { get; set; }

        /// <summary>
        ///     Gets or sets the list price Id.
        /// </summary>
        public string PriceListId { get; set; }

        public string SellingMethod { get; set; }

        public string UnitOfMeasure { get; set; }

        public bool IsUnit
        {
            get
            {
                return SellingMethod == "Unit";
            }
        }

        public bool IsUnitMeasure
        {
            get
            {
                return UnitOfMeasure == "Unit";
            }
        }

        public bool IsApproxUnit
        {
            get
            {
                return IsUnit && !IsUnitMeasure;
            }
        }

    }
}
