using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class PaymentViewModel : BaseViewModel
    {
        /// <summary>
        /// Id of the payment in the system.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The payment is locked when it required a Payment void to be modified.
        /// This impacts the Billing Address and the Payment Authorization.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Status of the current payment.
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// The PaymentMethod Info.
        /// </summary>
        public IPaymentMethodViewModel PaymentMethod { get; set; }

        /// <summary>
        /// The BillingAddress Info.
        /// </summary>
        public BillingAddressViewModel BillingAddress { get; set; }
    }
}
