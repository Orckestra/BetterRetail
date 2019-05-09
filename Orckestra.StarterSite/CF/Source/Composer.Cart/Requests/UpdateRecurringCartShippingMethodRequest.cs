using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Requests
{
    public class UpdateRecurringCartShippingMethodRequest
    {
        public string ShippingProviderId { get; set; }
        public string ShippingMethodName { get; set; }
        public string CartName { get; set; }
    }
}
