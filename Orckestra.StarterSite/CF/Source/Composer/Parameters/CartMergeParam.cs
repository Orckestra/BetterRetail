using System;

namespace Orckestra.Composer.Parameters
{
    public class CartMergeParam
    {
        /// <summary>
        /// The id of the guest customer 
        /// </summary>
        public Guid GuestCustomerId { get; set; }
        
        /// <summary>
        /// The id of the logged customer
        /// </summary>
        public Guid LoggedCustomerId { get; set; }

        /// <summary>
        /// The ScopeId where to find the cart
        /// Required
        /// </summary>
        public string Scope { get; set; }
    }
}
