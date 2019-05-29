using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    public sealed class CustomerSavedCreditCardPaymentMethodViewModel : BaseViewModel, ICustomerPaymentMethodViewModel
    {
        /// <summary>
        /// The Overture unique identifier of the Payment Method.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Overture unique identifier of the Payment Provider.
        /// </summary>
        public string PaymentProviderName { get; set; }

        /// <summary>
        /// The Name of the Payment Method.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Determine if the payment method is the default one
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Indicate the payment type
        /// </summary>
        public string PaymentType { get; set; }


        /// <summary>
        /// Indicate whether the payment method is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Actual card mask of the saved payment usually in the form of [XXXX **** **** XXXX]
        /// </summary>
        public string CardMask { get; set; }

        /// <summary>
        /// Card type (Mastercard/Visa/etc)
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// Expiration date of the card
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// Card holder name
        /// </summary>
        public string CardHolderName { get; set; }

        /// <summary>
        /// Indicate whether the card has expired or not.
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// Indicate whether the card is valid or not.
        /// </summary>
        public bool IsValid => !IsExpired;
    }
}
