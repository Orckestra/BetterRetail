using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class UpdateOrderParam
    {
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the Id of the order.
        /// </summary>
        /// <value>
        /// The Order Id.
        /// </value>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Updated order entity.
        /// </summary>
        public Overture.ServiceModel.Orders.Order Order { get; set; }
    }
}
