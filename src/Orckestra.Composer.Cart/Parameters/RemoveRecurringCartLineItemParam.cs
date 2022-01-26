using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RemoveRecurringCartLineItemParam
    {
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public Guid LineItemId { get; set; }
        public string CartName { get; set; }
        public Guid CustomerId { get; set; }
        public string BaseUrl { get; set; }
    }
}
