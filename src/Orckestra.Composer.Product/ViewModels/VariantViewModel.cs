using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    //TODO: see if there is a need for VariantViewModel. Actually it should end up quite similar to ProductDetailViewModel
    public sealed class VariantViewModel : BaseViewModel
    {
        public VariantViewModel()
        {
            SelectedImage = new ProductDetailImageViewModel();
            Images = new List<ProductDetailImageViewModel>();
            Kvas = new Dictionary<string, object>();
        }

        public string Id { get; set; }

        public string Sku { get; set; }

        [Formatting("General", "PriceFormat")]
        public string DisplayListPrice { get; set; }

        public bool IsOnSale { get; set; }

        /// <summary>
        /// Default image for this Variant
        /// </summary>
        public ProductDetailImageViewModel SelectedImage { get; set; }

        /// <summary>
        /// All images for this Variant
        /// </summary>
        public List<ProductDetailImageViewModel> Images { get; set; }

        public string FallbackImageUrl { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// Key variant attributes values for this Variant
        /// The Key is the kva property name
        /// The Value is the kva value for this variant
        /// </summary>
        public Dictionary<string,object> Kvas { get; set; }

        public SpecificationsViewModel Specifications { get; set; }
    }
}
