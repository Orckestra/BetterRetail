using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class RelatedProductViewModel : BaseProductViewModel
    {
        //Not a lookup in this case.
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

    }
}
