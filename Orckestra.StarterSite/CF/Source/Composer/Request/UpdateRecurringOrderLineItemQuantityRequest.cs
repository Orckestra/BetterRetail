using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Requests
{
    public class UpdateRecurringOrderLineItemQuantityRequest : BaseViewModel
    {
        public string RecurringLineItemId { get; set; }
        public double Quantity { get; set; }
    }
}
