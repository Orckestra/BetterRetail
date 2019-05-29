using Orckestra.Composer.ViewModels;
using System;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    public sealed class CustomerPaymentMethodViewModel : BaseViewModel, ICustomerPaymentMethodViewModel
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

        public bool IsValid { get; }
    }
}
