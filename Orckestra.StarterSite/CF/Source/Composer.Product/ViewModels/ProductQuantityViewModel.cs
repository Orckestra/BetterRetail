using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.ViewModels
{
    public sealed class ProductQuantityViewModel : BaseViewModel
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public int Value { get; set; }
    }
}