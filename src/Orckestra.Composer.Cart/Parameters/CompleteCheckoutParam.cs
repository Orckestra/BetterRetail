using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class CompleteCheckoutParam
    {
        /// <summary>
        /// The ScopeId where to complete the checkout
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
        /// The base Url associated with the current page
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
