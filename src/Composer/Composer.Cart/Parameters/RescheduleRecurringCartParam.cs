using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class RescheduleRecurringCartParam
    {
        public string Scope { get; set; }
        public string CartName { get; set; }
        public DateTime NextOccurence { get; set; }
        public Guid CustomerId { get; set; }
    }
}
