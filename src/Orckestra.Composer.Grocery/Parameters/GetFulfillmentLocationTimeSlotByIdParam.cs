using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class GetFulfillmentLocationTimeSlotByIdParam
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
        /// The type of the fulfillment
        /// </summary>
        public FulfillmentMethodType FulfillmentMethodType { get; set; }
    }
}