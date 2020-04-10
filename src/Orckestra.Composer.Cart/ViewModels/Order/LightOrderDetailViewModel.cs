using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class LightOrderDetailViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the order informations.
        /// </summary>
        public OrderDetailInfoViewModel OrderInfos { get; set; }

        public List<OrderShipmentDetailViewModel> Shipments { get; set; }

        /// <summary>
        /// Gets or sets the order URL.
        /// </summary>
        /// <value>
        /// The order URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Get or sets the list of shipments
        /// </summary>
        public List<OrderShipmentSummaryViewModel> ShipmentSummaries { get; set; }

        public LightOrderDetailViewModel()
        {
            ShipmentSummaries = new List<OrderShipmentSummaryViewModel>();
        }
    }
}
