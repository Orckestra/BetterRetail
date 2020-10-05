using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class ReserveTimeSlotParam
    {
        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The unique identifier of the time slot
        /// </summary>
        public Guid SlotId { get; set; }

        /// <summary>
        /// The cart name associated with the reservation
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The cart customer id associated with the reservation
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The reservation date
        /// </summary>
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// The expiry time for this reservation
        /// </summary>
        public TimeSpan ExpiryTime { get; set; }

        /// <summary>
        /// The expiry warning time for this reservation
        /// </summary>
        public TimeSpan ExpiryWarningTime { get; set; }

        /// <summary>
        /// The unique identifier of the Shipment
        /// </summary>
        public Guid ShipmentId { get; set; }

        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}