using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class CalculateScheduleAvailabilitySlotsParam
    {
        public string Scope { get; set; }

        /// <summary>
        /// The date of the first day to calculate for
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date of the last day to calculate for
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The unique identifier of the Order
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// The unique identifier of the Shipment
        /// </summary>
        public Guid ShipmentId { get; set; }

        /// <summary>
        /// The unique identifier of the Fulfillment location
        /// </summary>
        public Guid FulfillmentLocationId { get; set; }

        /// <summary>
        /// The type of the fulfillment
        /// </summary>
        public FulfillmentMethodType FulfillmentType { get; set; }

        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}