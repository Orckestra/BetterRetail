using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetCustomerOrderParam : GetOrderByNumberParam
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        public Guid CustomerId { get; set; }

       
    }
}
