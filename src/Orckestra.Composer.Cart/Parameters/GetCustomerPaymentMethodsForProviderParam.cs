using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCustomerPaymentMethodsForProviderParam
    {
        public Guid CustomerId { get; set; }
        public string ScopeId { get; set; }
        public string ProviderName { get; set; }
    }
}
