using System;

namespace Orckestra.Composer.Cart.Parameters
{
    public class VoidOrRemovePaymentParam : BaseCartParam
    {
        /// <summary>
        /// Id of the payment to void.
        /// </summary>
        public Guid PaymentId { get; set; }
    }
}