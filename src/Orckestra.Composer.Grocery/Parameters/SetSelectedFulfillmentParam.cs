using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class SetSelectedFulfillmentParam : SetSelectedStoreParam
    {
        /// <summary>
        /// The type of the fulfillment
        /// </summary>
        public FulfillmentMethodType? FulfillmentMethodType { get; set; }

        /// <summary>
        /// The ScopeId where to find the cart
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The name associated to the requested cart
        /// Required
        /// </summary>
        public string CartName { get; set; }
    }
}
