using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class UpdateCartResultViewModel : BaseViewModel
    {
        public bool HasErrors { get; set; }

        public string NextStepUrl { get; set; }

        public CartViewModel Cart { get; set; }
    }
}
