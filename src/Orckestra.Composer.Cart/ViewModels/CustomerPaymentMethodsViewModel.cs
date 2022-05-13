using Orckestra.Composer.ViewModels;
using System.Collections.Generic;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class CustomerPaymentMethodsViewModel : BaseViewModel
    {
        public CustomerPaymentMethodsViewModel()
        {
            SavedCreditCards = new List<SavedCreditCardPaymentMethodViewModel>();
        }
        public List<SavedCreditCardPaymentMethodViewModel> SavedCreditCards { get; set; }

        /// <summary>
        /// Url to add a credit card to the customer's wallet profile
        /// </summary>
        public string AddWalletUrl { get; set; }
    }
}
