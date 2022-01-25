using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetPaymentParam : BaseCartParam
    {
        /// <summary>
        /// Id of the payment to void.
        /// </summary>
        public Guid PaymentId { get; set; }
    }
}