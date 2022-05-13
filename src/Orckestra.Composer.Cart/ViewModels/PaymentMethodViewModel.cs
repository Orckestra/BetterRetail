using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class PaymentMethodViewModel : BaseViewModel, IPaymentMethodViewModel
    {
        /// <summary>
        /// The Overture unique identifier of the Payment Method.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Overture unique identifier of the Payment Provider.
        /// </summary>
        public string PaymentProviderName { get; set; }

        public string PaymentProviderType { get; set; }

        /// <summary>
        /// The Name of the Payment Method.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Determines if the payment method is currently selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Determine if the payment method is the default one
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Indicate the payment type
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// Indicate whether the card is valid or not.
        /// </summary>
        public bool IsValid => true;

        public bool IsCreditCardPaymentMethod { get; set; }
    }
}
