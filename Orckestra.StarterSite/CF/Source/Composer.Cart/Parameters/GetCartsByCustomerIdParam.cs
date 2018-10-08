using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCartsByCustomerIdParam
    {
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
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// A value indicating whether to include carts found in child scopes
        /// Optional 
        /// false by default
        /// </summary>
        public bool IncludeChildScopes { get; set; }
    }
}
