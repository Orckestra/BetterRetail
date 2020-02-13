using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class RemovePaymentMethodViewModel : BaseViewModel
    {
        public Guid PaymentMethodId { get; set; }

        public string PaymentProviderName { get; set; }
    }
}
