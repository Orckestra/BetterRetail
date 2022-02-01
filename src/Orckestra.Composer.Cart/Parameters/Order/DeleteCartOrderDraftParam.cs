using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class DeleteCartOrderDraftParam
    {
        /// <summary>
        /// The id of the requested scope
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The unique identifier of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The unique identifier of the order.
        /// </summary>
        public Guid OrderId {get;set;}
    }
}
