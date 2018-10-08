using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderShipmentDetailViewModel : BaseViewModel
    {
        /// <summary>
        /// The index of the shipment in the shipment list (non zero based)
        /// </summary>
        public string Index { get; set; }

        public List<LineItemDetailViewModel> LineItems { get; set; }

        /// <summary>
        /// Gets or sets the tracking infos.
        /// </summary>
        public TrackingInfoViewModel TrackingInfo { get; set; }

        /// <summary>
        /// Gets or sets the scheduled ship date.
        /// </summary>
        /// <value>
        /// The scheduled ship date.
        /// </value>
        public string ScheduledShipDate { get; set; }

        /// <summary>
        /// Gets or sets the Shipment notes
        /// </summary>
        public List<string> Comments { get; set; }

        /// <summary>
        /// Gets or sets the order status name.
        /// </summary>
        /// <value>
        /// The order status.
        /// </value>
        public string ShipmentStatusName { get; set; }

        /// <summary>
        /// Gets or sets the order status name.
        /// </summary>
        /// <value>
        /// The order status.
        /// </value>
        public string ShipmentStatus { get; set; }

        /// <summary>
        /// gets or sets the last date of the current status
        /// </summary>
        public string ShipmentStatusDate { get; set; }

        /// <summary>
        /// Gets or sets the history of the shipment level
        /// </summary>
        public List<OrderChangeViewModel> History { get; set; }

        /// <summary>
        /// Gets or sets the list of Discounts viewmodel.
        /// </summary>
        public List<RewardViewModel> Rewards { get; set; }

        /// <summary>
        /// Gets or sets the shipping address viewmodel.
        /// </summary>
        public AddressViewModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the Shipping method viewmodel for this shipment.
        /// </summary>
        public OrderShippingMethodViewModel ShippingMethod { get; set; }

        public OrderShipmentDetailViewModel()
        {
            LineItems = new List<LineItemDetailViewModel>();
            Comments = new List<string>();
            Rewards = new List<RewardViewModel>();
        }
    }
}
