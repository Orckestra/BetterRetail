using System;
using System.Collections.Generic;
using Orckestra.Composer.ViewModels;
using System.Linq;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.ViewModels
{
    public sealed class CustomerPaymentViewModel : BaseViewModel
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
        /// The List of PaymentMethodViewModel.
        /// </summary>
        public IList<ICustomerPaymentMethodViewModel> PaymentMethods { get; set; }

        /// <summary>
        /// Determines if the control is loading.
        /// </summary>
        public bool IsLoading { get; set; }

        public IEnumerable<CustomerSavedCreditCardPaymentMethodViewModel> SavedCreditCards
        {
            get { return PaymentMethods.OfType<CustomerSavedCreditCardPaymentMethodViewModel>(); }
        }

        public ICustomerPaymentMethodViewModel CreditCardPaymentMethod
        {
            get
            {
                return PaymentMethods.FirstOrDefault(paymentMethod =>
                    paymentMethod.PaymentType.Equals(PaymentMethodType.CreditCard.ToString(), StringComparison.OrdinalIgnoreCase)
                );
            }
        }
    }
}
