using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.OccClient
{
    public static class PaymentStatus
    {
        public const string New = "New";

        public const string PendingVerification = "PendingVerification";
        public const string VerificationFailed = "VerificationFailed";
        public const string Verified = "Verified";

        public const string PendingAuthorization = "PendingAuthorization";
        public const string AuthorizationFailed = "AuthorizationFailed";
        public const string Authorized = "Authorized";

        public const string Paid = "Paid";
        public const string PaymentFailed = "PaymentFailed";

        public const string OnHoldSettlement = "OnHold";
        public const string PendingVoid = "PendingVoid";
        public const string Voided = "Voided";
        public const string Refunded = "Refunded";
        public const string RefundFailed = "RefundFailed";

        public const string PartiallyRefunded = "PartiallyRefunded";

        /// <summary>
        /// Indicating that a payment has been aborted.
        /// </summary>
        /// <returns>True if payment status is PendingVoid, Voided or Refunded.</returns>
        public static bool IsCancelled(string paymentStatus)
        {
            return paymentStatus == PendingVoid ||
                   paymentStatus == Voided ||
                   paymentStatus == Refunded;
        }
    }
}
