using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Requests
{
    public class UpdateRecurringTemplatePaymentMethodRequest
    {
        public string ShippingProviderId { get; set; }
        public string ShippingMethodName { get; set; }
    }
}
