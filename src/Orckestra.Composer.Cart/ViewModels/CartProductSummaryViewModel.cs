using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CartProductSummaryViewModel : BaseViewModel
    {
        /// <summary>
        /// The display name of the product
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Id of the primary category of the product, if available.
        /// </summary>
        [MapTo("PrimaryParentCategoryId")]
        public string CategoryId { get; set; }

        [Lookup(LookupType.Product, "Brand")]
        public string Brand { get; set; }

        [MapTo("Brand")]
        public string BrandId { get; set; }


    }
}