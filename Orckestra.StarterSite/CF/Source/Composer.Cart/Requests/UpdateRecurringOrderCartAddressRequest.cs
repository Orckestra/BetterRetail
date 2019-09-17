using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Requests
{
    public class UpdateRecurringOrderCartAddressRequest
    {
        public string ShippingAddressId { get; set; }
        public string BillingAddressId { get; set; }
        public bool UseSameForShippingAndBilling { get; set; }
        public string cartName { get; set; }

    }
}
