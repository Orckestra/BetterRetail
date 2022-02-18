using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{

    /// <summary>
    /// Parameters for deleting an item to the cart
    /// </summary>
    public class RemoveLineItemParam : BaseCartParam
    {
        /// <summary>
        /// The ScopeId where to find the cart
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The Request Base Url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The culture for returned cart info
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The customer id to who belong the cart
        /// Required
        /// </summary>
        public Guid CustomerId { get; set; }


        /// <summary>
        /// The unique identifier of the LineItem to be removed
        /// Required
        /// </summary>
        public Guid LineItemId { get; set; }


        /// <summary>
        /// The name of the cart where to delete item
        /// Required
        /// </summary>
        public string CartName { get; set; }

        public RemoveLineItemParam Clone()
        {
            var param = (RemoveLineItemParam)MemberwiseClone();
            return param;
        }
    }
}

