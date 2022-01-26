using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public abstract class BaseCartParam
    {
        /// <summary>
        /// The ScopeId in which the cart is
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

        /// <summary>
        /// The cart type
        /// </summary>
        public string CartType { get; set; }
    }
}
