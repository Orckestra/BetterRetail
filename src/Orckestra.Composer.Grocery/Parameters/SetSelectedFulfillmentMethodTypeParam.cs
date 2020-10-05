using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class SetSelectedFulfillmentMethodTypeParam
    {
        /// <summary>
        /// The type of the fulfillment
        /// </summary>
        public FulfillmentMethodType FulfillmentMethodType { get; set; }

        /// <summary>
        /// The ScopeId where to find the cart
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The unique identifier of the Customer owning the cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The name associated to the requested cart
        /// Required
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
