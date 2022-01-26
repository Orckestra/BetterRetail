using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCustomerPaymentMethodListViewModelParam
    {
        public string ScopeId { get; set; }
        public CultureInfo CultureInfo{ get; set; }
        public Guid CustomerId { get; set; }
        public List<string> ProviderNames { get; set; }
    }
}
