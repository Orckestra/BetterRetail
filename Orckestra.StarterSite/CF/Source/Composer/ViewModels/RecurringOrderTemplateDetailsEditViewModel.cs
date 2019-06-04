using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels
{
    public class RecurringOrderTemplateDetailsEditViewModel : BaseViewModel
    {
        public RecurringOrderTemplateDetailsEditViewModel()
        {
        }

        public RecurringOrderTemplateLineItemViewModel RecurringOrderTemplateLineItemViewModel { get; set; }

        public RecurringOrdersTemplatesShippingMethodsViewModel AvailableShippingMethods { get; set; }
       // public AddressListViewModel AvailableAddresses { get; set; }              

       // public List<SavedCreditCard> AvailablePaymentMethods { get; set }
    }
}
