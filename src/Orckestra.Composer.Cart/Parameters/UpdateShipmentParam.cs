using System;
using System.Globalization;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class UpdateShipmentParam
    {
        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ScopeId { get; set; }

        /// <summary>
        /// The culture name in which language the data will be returned.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The name associated to the requested cart", IsRequired = true
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The unique identifier of the Customer owning the cart", IsRequired = true
        /// </summary>
        public Guid CustomerId { get; set; }

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
    }
}
