﻿using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Providers.Payment
{
    /// <summary>
    /// Composer Payment Provider for Moneris Canada.
    /// </summary>
    public class BamboraPaymentProvider : IPaymentProvider
    {
        /// <summary>
        /// Provider type name
        /// </summary>

        public const string ProviderTypeName = "ExternalPaymentProvider";

        protected IPaymentRepository PaymentRepository { get; private set; }
        protected virtual ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected virtual ILocalizationProvider LocalizationProvider { get; private set; }
        protected ICurrencyProvider CurrencyProvider { get; private set; }

        /// <summary>
        /// Type of Overture Payment Provider.
        /// </summary>
        public string ProviderType { get { return ProviderTypeName; } }

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string ProviderName { get; set; }

        public BamboraPaymentProvider(IPaymentRepository paymentRepository, ICartViewModelFactory cartViewModelFactory, 
            ILocalizationProvider localizationProvider, ICurrencyProvider currencyProvider)
        {
            PaymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            CurrencyProvider = currencyProvider ?? throw new ArgumentNullException(nameof(currencyProvider));
        }

        /// <summary>
        /// Initializes the payment based on the PaymentProvider logic.
        /// </summary>
        /// <param name="cart">Cart.</param>
        /// <param name="param">Parameters used to make the request.</param>
        /// <returns></returns>
        public Task<Overture.ServiceModel.Orders.Cart> InitializePaymentAsync(Overture.ServiceModel.Orders.Cart cart, InitializePaymentParam param)
        {
            if (!string.IsNullOrWhiteSpace(param.PaymentType) && Enum.TryParse(param.PaymentType, out PaymentMethodType paymentMethodType))
            {
                if (paymentMethodType == PaymentMethodType.SavedCreditCard)
                {
                    return Task.FromResult(cart);
                }
            }
            return PaymentRepository.InitializePaymentAsync(param);
        }

        /// <summary>
        /// Determines if the payment information should be captured.
        /// </summary>
        /// <param name="payment">Payment information.</param>
        /// <returns>True if the payment should be captured. False otherwise.</returns>
        public virtual bool ShouldCapturePayment(Overture.ServiceModel.Orders.Payment payment)
        {
            //TODO: There may be cases where we'll want to not capture the payment.
            return true;
        }

        /// <summary>
        /// Gets the URL to capture the payment information.
        /// </summary>
        /// <param name="payment">Payment information.</param>
        /// <param name="cultureInfo">CultureInfo of the request.</param>
        /// <returns>URL to capture the payment information.</returns>
        public virtual string GetCapturePaymentUrl(Overture.ServiceModel.Orders.Payment payment, CultureInfo cultureInfo)
        {
            return string.Empty;
        }

        protected virtual NameValueCollection FilterQueryString(NameValueCollection sourceQueryString, CultureInfo cultureInfo)
        {
            var finalQueryString = new NameValueCollection();

            foreach (string key in sourceQueryString.AllKeys)
            {
                if (!key.StartsWith("?", StringComparison.OrdinalIgnoreCase)
                    && cultureInfo.CompareInfo.IndexOf(key, "css", CompareOptions.IgnoreCase) < 0)
                {
                    finalQueryString.Add(key, sourceQueryString[key]);
                }
            }

            return finalQueryString;
        }

        /// <summary>
        /// Augments the ViewModel with useful data.
        /// </summary>
        /// <param name="viewModel">The ViewModel to augment.</param>
        /// <param name="payment">The payment.</param>
        public virtual void AugmentViewModel(ActivePaymentViewModel viewModel, Overture.ServiceModel.Orders.Payment payment) { }

        public virtual OrderSummaryPaymentViewModel BuildOrderSummaryPaymentViewModel(Overture.ServiceModel.Orders.Payment payment, CultureInfo cultureInfo)
        {
             var paymentMethodName = string.Empty;

            if (payment.PaymentMethod.PropertyBag != null)
            {
               
            }

            var paymentVm = new OrderSummaryPaymentViewModel
            {
                FirstName = payment.BillingAddress.FirstName,
                LastName = payment.BillingAddress.LastName,
                PaymentMethodName = paymentMethodName
            };

            paymentVm.BillingAddress = CartViewModelFactory.GetAddressViewModel(payment.BillingAddress, cultureInfo);
            paymentVm.Amount = LocalizationProvider.FormatPrice(payment.Amount, CurrencyProvider.GetCurrency());

            return paymentVm;
        }
    }
}