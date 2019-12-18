using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemovePaymentMethodParam
    {
        public Guid PaymentMethodId { get; set; }

        public string ScopeId { get; set; }

        public Guid CustomerId { get; set; }

        public string CartName { get; set; }

        public string PaymentProviderName { get; set; }
    }
}
