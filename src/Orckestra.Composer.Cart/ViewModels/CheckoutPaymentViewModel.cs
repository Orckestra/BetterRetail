using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;
using System.Linq;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CheckoutPaymentViewModel : BaseViewModel
    {
        private static string[] OnlinePaymentMethods = new[]
        {
            PaymentMethodType.CreditCard.ToString(),            
            PaymentMethodType.SavedCreditCard.ToString(),
        };

        private static string[] UponReceptionPaymentMethods = new[]
        {
            PaymentMethodType.Cash.ToString(),
            PaymentMethodType.CashCard.ToString(),
            PaymentMethodType.Cheque.ToString(),
            PaymentMethodType.Debit.ToString(),
            PaymentMethodType.GiftCertificate.ToString(),
            PaymentMethodType.OnSiteCredit.ToString(),
            PaymentMethodType.OnSiteDebit.ToString(),
            PaymentMethodType.OnSiteUnspecified.ToString(),
            PaymentMethodType.PurchaseOrder.ToString(),            
        };

        /// <summary>
        /// Id of the payment to be updated.
        /// </summary>
        public Guid? PaymentId { get; set; }

        /// <summary>
        /// The List of PaymentMethodViewModel.
        /// </summary>
        public IList<IPaymentMethodViewModel> PaymentMethods { get; set; }

        /// <summary>
        /// The List of PaymentProviderViewModel.
        /// </summary>
        public IList<PaymentProviderViewModel> PaymentProviders { get; set; }

        /// <summary>
        /// Determines if the control is loading.
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// ViewModel of the active payment.
        /// </summary>
        public ActivePaymentViewModel ActivePaymentViewModel { get; set; }

        /// <summary>
        /// ViewModel of credit card trust image.
        /// </summary>
        public ImageViewModel CreditCardTrustImage { get; set; }

        public bool HaveMonerisPaymentProvider
        {
            get
            {
                if (PaymentMethods?.FirstOrDefault(p => p.PaymentProviderType == "MonerisCanadaPaymentProvider") != null)
                {
                    return true;
                }
                return false;
            }
        }

        public IEnumerable<SavedCreditCardPaymentMethodViewModel> SavedCreditCards
        {
            get { return PaymentMethods.OfType<SavedCreditCardPaymentMethodViewModel>(); }
        }

        public bool IsSavedCreditCardSelected
        {
            get { return SavedCreditCards.Any(savedCreditCard => savedCreditCard.IsSelected); }
        }

        public IPaymentMethodViewModel CreditCardPaymentMethod
        {
            get
            {
                return PaymentMethods.FirstOrDefault(paymentMethod =>
                    paymentMethod.PaymentType.Equals(PaymentMethodType.CreditCard.ToString(), StringComparison.OrdinalIgnoreCase)
                );
            }
        }

        public bool IsCreditCardSelected
        {
            get
            {
                if (CreditCardPaymentMethod != null)
                {
                    return CreditCardPaymentMethod.IsSelected;
                }
                return false;
            }
        }

        public IEnumerable<IPaymentMethodViewModel> UponReceptionPaymentMethodViewModels
        {
            get
            {
                return PaymentMethods.Where(paymentMethod =>
                    UponReceptionPaymentMethods.Contains(paymentMethod.PaymentType, StringComparer.OrdinalIgnoreCase)
                );
            }
        }

        public CheckoutPaymentViewModel()
        {
            PaymentMethods = new List<IPaymentMethodViewModel>();
            PaymentProviders = new List<PaymentProviderViewModel>();
        }
    }
}
