using System;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateShipmentParam : BaseCartParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The unique identifier of the Shipment to update.", IsRequired = true
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Shipment.ShippingAddress to update.
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// The unique identifier of the shipping provider.
        /// </summary>
        public Guid ShippingProviderId { get; set; }

        /// <summary>
        /// The fulfillment location id.
        /// </summary>
        public Guid FulfillmentLocationId { get; set; }

        /// <summary>
        /// The unique identifier of the Shipment.FulfillmentMethod.
        /// </summary>
        public string FulfillmentMethodName { get; set; }

        /// <summary>
        /// The property bag containing extended properties for this shipment.
        /// </summary>
        public PropertyBag PropertyBag { get; set; }

        /// <summary>
        /// the requested schedule begin date and time.
        /// </summary>
        public DateTime? FulfillmentScheduledTimeBeginDate { get; set; }

        /// <summary>
        /// the requested schedule end date and time.
        /// </summary>
        public DateTime? FulfillmentScheduledTimeEndDate { get; set; }

        /// <summary>
        /// The fulfillment schedule mode.
        /// </summary>
        public FulfillmentScheduleMode FulfillmentScheduleMode { get; set; }

        /// <summary>
        /// Gets or sets the pick-up location identifier required when the selected shipping method type is ship to store; any value will be ignored otherwise.
        /// </summary>
        public Guid? PickUpLocationId { get; set; }
    }
}
