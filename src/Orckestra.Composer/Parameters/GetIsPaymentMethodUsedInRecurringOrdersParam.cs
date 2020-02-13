using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Requests
{
    public class GetIsPaymentMethodUsedInRecurringOrdersRequest
    {
        public string PaymentMethodId { get; set; }
        public string ScopeId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public Guid CustomerId { get; set; }
    }
}
