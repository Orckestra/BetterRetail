using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Cart.ViewModels
{
    /// <summary>
    /// Recurring Order Template viewmodel
    /// </summary>
    public class RecurringOrderTemplateViewModel : BaseViewModel
    {
        public RecurringOrderTemplateViewModel()
        {
            RecurringOrderTemplateLineItemViewModels = new List<RecurringOrderTemplateLineItemViewModel>();
        }

        /// <summary>
        /// The address of the client of the first shipment.
        /// </summary>
        public AddressViewModel ShippingAddress { get; set; }

        /// <summary>
        /// The ShippingMethod of the first shipment.
        /// </summary>
        public ShippingMethodViewModel ShippingMethod { get; set; }

        /// <summary>
        /// The Payment info.
        /// </summary>
        public PaymentViewModel Payment { get; set; }

        /// <summary>
        /// List of line items (different products)
        /// </summary>
        public List<RecurringOrderTemplateLineItemViewModel> RecurringOrderTemplateLineItemViewModels { get; set; }
    }
}
