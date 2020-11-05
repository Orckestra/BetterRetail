using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class RelatedProductViewModel : BaseProductViewModel
    {
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
        /// <summary>
        /// todo
        /// </summary>
        public ProductQuantityViewModel Quantity { get; set; }

    }
}
