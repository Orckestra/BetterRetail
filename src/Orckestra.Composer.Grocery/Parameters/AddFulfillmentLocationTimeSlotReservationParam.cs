using Orckestra.Overture.ServiceModel.Orders;
using System;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class AddFulfillmentLocationTimeSlotReservationParam
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
        /// The unique identifier of the Fulfillment location
        /// </summary>
        public Guid FulfillmentLocationId { get; set; }

        /// <summary>
        /// The cart name associated with the reservation
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The cart customer id associated with the reservation
        /// </summary>
        public Guid CartCustomerId { get; set; }

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
        /// The reservation status. Options are Tentative, Confirmed, Expired and Void.
        /// </summary>
        public TimeslotReservationStatus ReservationStatus { get; set; }
    }
}
