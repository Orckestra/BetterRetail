using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    public sealed class ProductPriceSearchViewModel : BaseViewModel
    {
        /// <summary>
        ///     Gets or sets the display price.
        /// </summary>
        public string DisplayPrice { get; set; }

        /// <summary>
        ///     Gets or sets the display special price.
        /// </summary>
        public string DisplaySpecialPrice { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating wether this instance has a range of price.
        /// </summary>
        public bool HasPriceRange { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is on sale.
        /// </summary>
        /// <remarks>
        ///     If true Price/DisplaySpecialPrice will contains the discount price and ListPrice/DisplayPrice will contains the regular price
        ///     If false Price/DisplaySpecialPrice will be set to null and List/DisplayPrice will contains the regular Price
        /// </remarks>
        public bool IsOnSale { get; set; }

        /// <summary>
        ///     Gets or sets the list price.
        /// </summary>
        public double? ListPrice { get; set; }

        /// <summary>
        ///     Gets or sets the price.
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        ///     Gets or sets the list price Id.
        /// </summary>
        public string PriceListId { get; set; }
    }
}