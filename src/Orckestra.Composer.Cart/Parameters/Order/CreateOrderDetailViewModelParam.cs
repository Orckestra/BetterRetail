using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class CreateOrderDetailViewModelParam
    {
        /// <summary>
        /// The Overture Order to use for the viewmodel creation.
        /// </summary>
        public Overture.ServiceModel.Orders.Order Order { get; set; }

        /// <summary>
        /// The Display names for all order statuses.
        /// </summary>
        public Dictionary<string, string> OrderStatuses { get; set; }

        /// <summary>
        /// The Display names for all order statuses.
        /// </summary>
        public Dictionary<string, string> ShipmentStatuses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Overture.ServiceModel.Orders.OrderHistoryItem> OrderChanges { get; set; }

        /// <summary>
        /// All the notes for each shipment.
        /// </summary>
        public Dictionary<Guid, List<string>> ShipmentsNotes { get; set; }

        /// <summary>
        /// Culture Info for the ViewModel.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The country Iso code of the order.
        /// </summary>
        public string CountryCode { get; set; }
       
        /// <summary>
        /// The informations for the product images viewmodel creation.
        /// </summary>
        public ProductImageInfo ProductImageInfo { get; set; }

        /// <summary>
        /// The base Url.
        /// </summary>
        public string BaseUrl { get; set; }

        public string OrderDetailBaseUrl { get; set; }

        public CreateOrderDetailViewModelParam()
        {
            OrderStatuses = new Dictionary<string, string>();
            ShipmentStatuses = new Dictionary<string, string>();
            ShipmentsNotes = new Dictionary<Guid, List<string>>();
            OrderChanges = new List<OrderHistoryItem>();
        }
    }
}
