using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Requests
{
    public class UpdateRecurringOrderCartShippingAddressRequest
    {
        public string ShippingAddressId { get; set; }
        public bool UseSameForShippingAndBilling { get; set; }

    }
}
