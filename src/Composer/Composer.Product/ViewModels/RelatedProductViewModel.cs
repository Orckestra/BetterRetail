using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class RelatedProductViewModel : BaseViewModel
    {
        /// <summary>
        /// The Id of the Product
        /// </summary>
        [MapTo("Id")]
        public string ProductId { get; set; }

        [Lookup(LookupType.Product, "Brand")]
        public string Brand { get; set; }

        [MapTo("Brand")]
        public string BrandId { get; set; }

        /// <summary>
        /// Id of the primary category id.
        /// </summary>
        [MapTo("PrimaryParentCategoryId")]
        public string CategoryId { get; set; }

        /// <summary>
        /// Product Sku
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Localized product desctiption
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Localized product name
        /// </summary>
        public string DisplayName { get; set; }

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
        /// The <see cref="ListPrice"/> locally formatted as a currency
        /// </summary>
        [Formatting("General", "PriceFormat")]
        [MapTo("ListPrice")]
        public string DisplayPrice { get; set; }

        /// <summary>
        /// The <see cref="Price"/> formatted as a local curency.
        /// This field cannot come from the Overture product,
        /// so it is not annotated with mapping attributes. It's value must be set manually.
        /// </summary>
        public string DisplaySpecialPrice { get; set; }
        
        /// <summary>
        /// A boolean to check if <see cref="Price"/> is less than <see cref="ListPrice"/>
        /// </summary>
        public bool IsOnSale
        {
            get { return Price < ListPrice; }
        }

        /// <summary>
        /// Link to the product details page
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A standard fallback image to use in the event <see cref="ImageUrl"/> isn't set.
        /// </summary>
        public string FallbackImageUrl { get; set; }

        /// <summary>
        /// Large image of the product, and optionally variant
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// If the product has variants
        /// </summary>
        public bool HasVariants { get; set; }

        /// <summary>
        /// If Product is available to sell
        /// </summary>
        public bool IsAvailableToSell { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        public ProductQuantityViewModel Quantity { get; set; }

        /// <summary>
        /// Is eligible if RecurringOrderProgramName is not null or empty and if recurring orders flag is enabled
        /// </summary>
        public bool IsRecurringOrderEligible { get; set; }
        /// <summary>
        /// Name of the recurring order program associated to the product
        /// </summary>
        public string RecurringOrderProgramName { get; set; }
    }
}
