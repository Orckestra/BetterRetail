using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class SetCustomerDefaultPaymentMethodViewModel : BaseViewModel
    {
        public string PaymentProviderName { get; set; }

        public Guid PaymentMethodId { get; set; }
    }
}
