using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Request
{
    public class RemoveRecurringOrderTemplateLineItemsRequest
    {
        public List<string> LineItemsIds { get; set; }
    }
}
