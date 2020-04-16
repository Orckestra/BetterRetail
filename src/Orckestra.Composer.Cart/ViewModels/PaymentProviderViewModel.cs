using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class PaymentProviderViewModel : BaseViewModel
    {
        public string ProviderName { get; set; }

        public string ProviderType { get; set; }
    }
}
