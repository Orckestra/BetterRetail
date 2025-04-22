using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class ProductDetailVideoViewModel : BaseViewModel
    {
        public string ProductId { get; set; }

        public string VariantId { get; set; }

        public int SequenceNumber { get; set; }

        public string VideoUrl { get; set; }

        public string FallbackImageUrl { get; set; }

        public bool Selected { get; set; }

        public string Alt { get; set; }

        public string Description { get; set; }
    }
}