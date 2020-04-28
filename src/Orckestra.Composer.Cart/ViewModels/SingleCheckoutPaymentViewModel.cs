using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class SingleCheckoutPaymentViewModel : BaseViewModel
    {
        /// <summary>
        /// The List of PaymentMethodViewModel.
        /// </summary>
        public IList<IPaymentMethodViewModel> PaymentMethods { get; set; }

        /// <summary>
        /// The List of PaymentProviderViewModel.
        /// </summary>
        public IList<PaymentProviderViewModel> PaymentProviders { get; set; }

        /// <summary>
        /// ViewModel of the active payment.
        /// </summary>
        public ActivePaymentViewModel ActivePaymentViewModel { get; set; }

        /// <summary>
        /// ViewModel of credit card trust image.
        /// </summary>
        public ImageViewModel CreditCardTrustImage { get; set; }

        public SingleCheckoutPaymentViewModel()
        {
            PaymentMethods = new List<IPaymentMethodViewModel>();
            PaymentProviders = new List<PaymentProviderViewModel>();
        }
    }
}
