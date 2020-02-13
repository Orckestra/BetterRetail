using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
    public  class UpdateRecurringOrderTemplateLineItemQuantityParam
    {
        public string RecurringLineItemId { get; set; }
        /// <summary>
        /// New quantity
        /// </summary>
        public double Quantity { get; set; }

        public string ScopeId { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public Guid CustomerId { get; set; }
        public string BaseUrl { get; set; }
    }
}
