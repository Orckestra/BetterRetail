using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCustomerPaymentMethodsForProviderParam
    {
        public Guid CustomerId { get; set; }
        public string ScopeId { get; set; }
        public string ProviderName { get; set; }
    }
}
