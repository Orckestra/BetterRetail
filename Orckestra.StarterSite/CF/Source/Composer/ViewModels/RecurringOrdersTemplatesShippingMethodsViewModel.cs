using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    public class RecurringOrdersTemplatesShippingMethodsViewModel
    {
        public IList<RecurringOrderShippingMethodViewModel> ShippingMethods { get; set; } = new List<RecurringOrderShippingMethodViewModel>();
    }
}
