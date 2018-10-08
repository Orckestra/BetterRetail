using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class SetDefaultCustomerPaymentMethodParam
    {
        public Guid CustomerId { get; set; }

        public string PaymentProviderName { get; set; }

        public Guid PaymentMethodId { get; set; }

        public string ScopeId { get; set; }

        public CultureInfo Culture { get; set; }
    }
}
