using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetShippingMethodsParam : BaseCartParam
    {
        /// <summary>
        /// Id of the shipment. The fulfillment method will be fetched from this shipment. Put this parameter to Guid.Empty
        /// if you want to retrieve all fulfillment methods.
        /// </summary>
        public Guid ShipmentId { get; set; }
    }
}
