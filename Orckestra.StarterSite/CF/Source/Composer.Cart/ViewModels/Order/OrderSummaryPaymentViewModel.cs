using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderSummaryPaymentViewModel : BaseViewModel
    {
        /// <summary>
        /// The lastname of the payment owner.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The first name of the payment owner.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The payment method name.
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// The credit card number (masked).
        /// </summary>
        public string CreditCardNumber { get; set; }

        /// <summary>
        /// the amount paid
        /// </summary>
        [Formatting("General", "PriceFormat")]
        public string Amount { get; set; }

        /// <summary>
        /// the creadit card's expiry date
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// Indicate whether the card has expired or not.
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// Billing address in the Payment
        /// </summary>
        public AddressViewModel BillingAddress { get; set; }
    }
}
