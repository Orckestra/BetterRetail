using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
    public class UpdateRecurringOrderTemplateLineItemParam
    {
        public string LineItemId { get; set; }
        public string RecurringOrderFrequencyName { get; set; }
        public string ShippingAddressId { get; set; }
        public string BillingAddressId { get; set; }
        public string PaymentMethodId { get; set; }
        public DateTime NextOccurence { get; set; }
        public string ShippingProviderId { get; set; }
        public string ShippingMethodName { get; set; }

        public string BaseUrl { get; set; }
        public string ScopeId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public Guid CustomerId { get; set; }
    }
}
