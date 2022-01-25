using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemovePaymentMethodParam : BaseCartParam
    {
        public Guid PaymentMethodId { get; set; }

        public string PaymentProviderName { get; set; }
    }
}
