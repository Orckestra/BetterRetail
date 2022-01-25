using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderDetailViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the Order Summary ViewModel.
        /// </summary>
        public OrderSummaryViewModel OrderSummary { get; set; }

        /// <summary>
        /// Gets or sets the order informations.
        /// </summary>
        public OrderDetailInfoViewModel OrderInfos { get; set; }

        /// <summary>
        /// Gets or sets the list of shipments viewmodel.
        /// </summary>
        public List<OrderShipmentDetailViewModel> Shipments { get; set; } 

        /// <summary>
        /// Gets or sets the history of the order category
        /// </summary>
        public List<OrderChangeViewModel> History { get; set; }

        /// <summary>
        /// Gets or sets the shipping address viewmodel.
        /// </summary>
        public AddressViewModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the billing address viewmodel.
        /// </summary>
        public AddressViewModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the Shipping method viewmodel.
        /// </summary>
        public OrderShippingMethodViewModel ShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets the list of payments viewmodel.
        /// </summary>
        public List<OrderSummaryPaymentViewModel> Payments { get; set; }

        public OrderDetailViewModel()
        {
            Shipments = new List<OrderShipmentDetailViewModel>();
            Payments = new List<OrderSummaryPaymentViewModel>();
        }
    }
}
