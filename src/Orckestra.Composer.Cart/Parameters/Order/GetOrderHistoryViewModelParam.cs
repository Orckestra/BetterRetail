using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetOrderHistoryViewModelParam
    {
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the Orders Result
        /// </summary>
        public OrderQueryResult OrderResult { get; set; }

        /// <summary>
        /// Gets or sets the Order Statuses
        /// </summary>
        public Dictionary<string, string> OrderStatuses { get; set; }

        /// <summary>
        /// The Base url for the order detail page.
        /// </summary>
        public string OrderDetailBaseUrl { get; set; }


        /// <summary>
        /// Gets or sets the TrackingInfoViewModel for each shipment of the order.
        /// </summary>
        public Dictionary<Guid, TrackingInfoViewModel> ShipmentsTrackingInfos { get; set; } 
    }
}
