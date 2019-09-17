using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
    public class RemoveRecurringOrderTemplateLineItemsParam
    {
        public string ScopeId { get; set; }
        public CultureInfo Culture { get; set; }
        public List<string> LineItemsIds { get; set; }
        public Guid CustomerId { get; set; }
        public string BaseUrl { get; set; }
    }
}
