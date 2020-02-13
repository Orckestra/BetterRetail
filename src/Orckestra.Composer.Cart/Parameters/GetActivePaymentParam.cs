using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetActivePaymentParam
    {
        /// <summary>
        /// The ScopeId where to find the associated cart
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The unique identifier of the Customer owning the associated cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The name associated to the requested associated cart
        /// Required
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        public bool IsAuthenticated { get; set; }
    }
}
