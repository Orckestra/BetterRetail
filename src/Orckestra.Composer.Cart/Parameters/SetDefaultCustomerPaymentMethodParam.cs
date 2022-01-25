using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class SetDefaultCustomerPaymentMethodParam : BaseCartParam
    {
        public string PaymentProviderName { get; set; }

        public Guid PaymentMethodId { get; set; }
    }
}