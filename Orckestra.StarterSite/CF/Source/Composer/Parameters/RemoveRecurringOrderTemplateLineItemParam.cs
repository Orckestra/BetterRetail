using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
    public class RemoveRecurringOrderTemplateLineItemParam
    {
        public string ScopeId { get; set; }
        public CultureInfo Culture { get; set; }
        public string LineItemId { get; set; }
        public Guid CustomerId { get; set; }
        public string BaseUrl { get; set; }
    }
}
