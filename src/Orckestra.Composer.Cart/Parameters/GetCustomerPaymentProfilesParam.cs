using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCustomerPaymentProfilesParam
    {
        public Guid CustomerId { get; set; }

        public string ScopeId { get; set; }
    }
}
