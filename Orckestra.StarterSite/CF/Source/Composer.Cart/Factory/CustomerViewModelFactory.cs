using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Orckestra.Composer.Cart.Factory
{
    public class CustomerViewModelFactory : ICustomerViewModelFactory
    {
        protected IViewModelMapper ViewModelMapper { get; private set; }

        public CustomerViewModelFactory(IViewModelMapper viewModelMapper)
        {
            if (viewModelMapper == null) { throw new ArgumentNullException("viewModelMapper"); }

            ViewModelMapper = viewModelMapper;
        }

        /// <summary>
        /// Gets a PaymentMethodViewModel from an Overture PaymentMethod object.
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <param name="paymentMethodDisplayNames"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public virtual ICustomerPaymentMethodViewModel GetPaymentMethodViewModel(
            PaymentMethod paymentMethod,
            Dictionary<string, string> paymentMethodDisplayNames,
            CultureInfo cultureInfo)
        {
            if (paymentMethod == null)
            {
                return null;
            }

            if (paymentMethodDisplayNames == null)
            {
                return null;
            }

            var paymentMethodDisplayName = paymentMethodDisplayNames.FirstOrDefault(x => x.Key == paymentMethod.Type.ToString()).Value;

            if (paymentMethodDisplayName == null)
            {
                return null;
            }

            ICustomerPaymentMethodViewModel paymentMethodViewModel;
            switch (paymentMethod.Type)
            {
                case PaymentMethodType.SavedCreditCard:
                    paymentMethodViewModel = MapSavedCreditCard(paymentMethod, cultureInfo);
                    break;

                default:
                    var vm = ViewModelMapper.MapTo<CustomerPaymentMethodViewModel>(paymentMethod, cultureInfo);
                    vm.DisplayName = paymentMethodDisplayName;
                    paymentMethodViewModel = vm;
                    break;
            }

            paymentMethodViewModel.PaymentType = paymentMethod.Type.ToString();

            return paymentMethodViewModel;
        }

        protected CustomerSavedCreditCardPaymentMethodViewModel MapSavedCreditCard(PaymentMethod payment, CultureInfo cultureInfo)
        {            
            var savedCreditCard = ViewModelMapper.MapTo<CustomerSavedCreditCardPaymentMethodViewModel>(payment, cultureInfo);

            if (!string.IsNullOrWhiteSpace(savedCreditCard.ExpiryDate))
            {
                var expirationDate = ParseCreditCartExpiryDate(savedCreditCard.ExpiryDate);
                expirationDate = expirationDate.AddDays(DateTime.DaysInMonth(expirationDate.Year, expirationDate.Month) - 1);
                savedCreditCard.IsExpired = expirationDate < DateTime.UtcNow;
            }

            return savedCreditCard;
        }

        protected virtual DateTime ParseCreditCartExpiryDate(string expiryDate)
        {
            var formats = new[]
            {
                "MMyy",
                "MM/yy",
                "MM-yy",
            };

            return DateTime.ParseExact(expiryDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }    
    }
}