using System;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class ActivePaymentViewModel : BaseViewModel
    {
        /// <summary>
        /// Id of the payment in the system.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Unique identifier of the payment method linked to the active payment
        /// </summary>
        public string PaymentMethodType { get; set; }

        public Guid PaymentMethodId { get; set; }

        /// <summary>
        /// Determines if the front-end need to capture the payment.
        /// </summary>
        public bool ShouldCapturePayment { get; set; }

        /// <summary>
        /// Url at which the Overture Payment Provider is configured to capture the payment information.
        /// </summary>
        public string CapturePaymentUrl { get; set; }

        /// <summary>
        /// Status of the Active Payment.
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Type of the Payment Provider.
        /// </summary>
        public string ProviderType { get; set; }

        /// <summary>
        /// Indicate wether it is possible to save the current payment method
        /// </summary>
        public bool CanSavePaymentMethod { get; set; }

        /// <summary>
        /// Indicate wether the payment method must be saved
        /// </summary>
        public bool MustSavePaymentMethod { get; set; }

        /// <summary>
        /// The Image Infos of the Payment trust icon.
        /// </summary>
        public ImageViewModel CreditCardTrustImage { get; set; }
    }
}
