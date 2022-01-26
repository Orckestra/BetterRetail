using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetShippingMethodsParam
    {
        /// <summary>
        /// The name associated to the cart to find shipping methods
        /// Required
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The unique identifier of the Customer owning the cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The ScopeId where to find the shipping methods
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The culture for returned Shipping methods info
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Id of the shipment. The fulfillment method will be fetched from this shipment. Put this parameter to Guid.Empty
        /// if you want to retrieve all fulfillment methods.
        /// </summary>
        public Guid ShipmentId { get; set; }
    }
}
