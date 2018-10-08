using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Extensions
{
    internal static class PaymentExtensions
    {
        public static bool IsVoided(this Payment payment)
        {
            if (payment == null) { throw new ArgumentNullException(nameof(payment)); }

            return payment.PaymentStatus == PaymentStatus.Voided;
        }

        /// <summary>
        /// Return the payment by id, if the payment does not exist or is voided an <see cref="InvalidOperationException"/> will be thrown
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="payments"></param>
        /// <returns></returns>
        public static Payment GetPayment(this IEnumerable<Payment> payments, Guid paymentId)
        {
            var activePayment = payments.FirstOrDefault(p => p.Id == paymentId);

            if (activePayment == null || activePayment.IsVoided())
            {
                throw new InvalidOperationException($"The payment you are trying to update ({paymentId}) is either not found or already voided.");
            }

            return activePayment;
        }

        /// <summary>
        /// Validate if the payment should invoke pre-payment switch
        /// </summary>
        /// <param name="activePayment"></param>
        /// <returns></returns>
        public static bool ShouldInvokePrePaymentSwitch(this Payment activePayment)
        {
            return activePayment.PaymentStatus != PaymentStatus.New;
        }
    }
}
