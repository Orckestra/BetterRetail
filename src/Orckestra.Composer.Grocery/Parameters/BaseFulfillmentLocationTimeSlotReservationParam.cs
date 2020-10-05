using System;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class BaseFulfillmentLocationTimeSlotReservationParam
    {
        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The unique identifier of the time slot reservation
        /// </summary>
        public Guid SlotReservationId { get; set; }

        /// <summary>
        /// The unique identifier of the Fulfillment location
        /// </summary>
        public Guid FulfillmentLocationId { get; set; }
    }
}