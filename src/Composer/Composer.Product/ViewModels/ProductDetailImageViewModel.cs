using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class ProductDetailImageViewModel : BaseViewModel
    {
        public string ProductId { get; set; }

        public string VariantId { get; set; }

        public int SequenceNumber { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ProductZoomImageUrl { get; set; }

        public string ImageUrl { get; set; }

        public string FallbackImageUrl { get; set; }

        public bool Selected { get; set; }

        public bool IsProductZoomImageUrlDefined { get; set; }

        public string Alt { get; set; }
    }
}
