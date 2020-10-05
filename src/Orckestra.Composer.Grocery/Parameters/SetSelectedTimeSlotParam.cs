using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class SetSelectedTimeSlotParam
    {
        /// <summary>
        /// The unique identifier of the time slot
        /// </summary>
        public Guid SlotId { get; set; }

        /// <summary>
        /// The day reserved time slot
        /// </summary>
        public DateTime Day { get; set; } 

        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The unique identifier of the shipment
        /// </summary>
        public Guid ShipmentId { get; set; }

        /// <summary>
        /// The cart name associated with the reservation
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The cart customer id associated with the reservation
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The unique identifier of the Fulfillment location
        /// </summary>
        public Guid FulfillmentLocationId { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The Timeslot Reservation Id
        /// </summary>
        public Guid TimeSlotReservationId { get; set; }
    }
}
