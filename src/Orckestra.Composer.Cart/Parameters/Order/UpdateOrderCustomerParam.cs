using System;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class UpdateOrderCustomerParam : GetOrderParam
    {
        /// <summary>
        /// The Customer Id for updated order owner.
        /// </summary>
        public Guid CustomerId { get; set; }
    }
}
