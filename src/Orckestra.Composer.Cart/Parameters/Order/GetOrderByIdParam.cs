using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetOrderByIdParam : GetOrderParam
    {
        /// <summary>
        /// Gets or sets the id of the order to retrieve.
        /// </summary>
        /// <value>
        /// The order id.
        /// </value>
        public Guid OrderId { get; set; }
    }
}
