using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Providers.Payment
{
    /// <summary>
    /// Composer Payment Provider for Moneris Canada.
    /// </summary>
    public class MonerisCanadaPaymentProvider : IPaymentProvider
    {
        /// <summary>
        /// Provider type name
        /// </summary>
        /// <remarks>
        /// The visibility have been changed to public to allow <see cref="Orckestra.Composer.Cart.Services.PaymentViewService.GetActivePaymentViewModel">PaymentViewService</see>
        /// reference its type name as part of the "Save Credit Card" logic, since this is only available for this provider
        /// </remarks>
        public const string MonerisProviderTypeName = "MonerisCanadaPaymentProvider";

        private readonly string SavedCardMask = "CardMask";
        private readonly string SavedCardType = "CardType";
        private readonly string SavedExpiryDate = "ExpiryDate";

        private readonly string HostedCardTokenizationUrl = "HostedCardTokenizationUrl";

        protected IPaymentRepository PaymentRepository { get; private set; }
        protected virtual ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected virtual ILocalizationProvider LocalizationProvider { get; private set; }

        /// <summary>
        /// Type of Overture Payment Provider.
        /// </summary>
        public string ProviderType { get { return MonerisProviderTypeName; } }

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string ProviderName { get; set; }

        public MonerisCanadaPaymentProvider(IPaymentRepository paymentRepository, ICartViewModelFactory cartViewModelFactory, 
            ILocalizationProvider localizationProvider)
        {
            PaymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
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
            if (payment.PaymentMethod == null || payment.PaymentMethod.PropertyBag == null || 
                !payment.PaymentMethod.PropertyBag.ContainsKey(HostedCardTokenizationUrl)) { return null;  }

            var url = payment.PaymentMethod.PropertyBag[HostedCardTokenizationUrl].ToString();

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri rawCaptureUri)) { return null; } //TODO: Throw here instead.

            var queryString = rawCaptureUri.ParseQueryString();
            var finalQueryString = FilterQueryString(queryString, cultureInfo);

            var finalUrl = UrlFormatter.AppendQueryString(rawCaptureUri.GetLeftPart(UriPartial.Path), finalQueryString);
            return finalUrl;
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
            var creditCartNumber = string.Empty;
            var expiryDate = string.Empty;
            var paymentMethodName = string.Empty;

            if (payment.PaymentMethod.PropertyBag != null)
            {
                //TODO : use viewmodelmapper through cartviewmodelfactory ( already exist )
                if (payment.PaymentMethod.Type == PaymentMethodType.SavedCreditCard)
                {
                    if (payment.PaymentMethod.PropertyBag.ContainsKey(SavedCardMask))
                    {
                        creditCartNumber = payment.PaymentMethod.PropertyBag[SavedCardMask].ToString();
                    }

                    if (payment.PaymentMethod.PropertyBag.ContainsKey(SavedExpiryDate))
                    {
                        expiryDate = payment.PaymentMethod.PropertyBag[SavedExpiryDate].ToString();
                    }

                    if (payment.PaymentMethod.PropertyBag.ContainsKey(SavedCardType))
                    {
                        paymentMethodName = payment.PaymentMethod.PropertyBag[SavedCardType].ToString();
                    }
                }
                else
                {
                    if (payment.PaymentMethod.PropertyBag.ContainsKey(CreditCardPaymentProperties.CreditCardNumberLastDigitsKey))
                    {
                        creditCartNumber = payment.PaymentMethod.PropertyBag[CreditCardPaymentProperties.CreditCardNumberLastDigitsKey].ToString();
                    }

                    if (payment.PaymentMethod.PropertyBag.ContainsKey(CreditCardPaymentProperties.CreditCardExpiryDateKey))
                    {
                        expiryDate = payment.PaymentMethod.PropertyBag[CreditCardPaymentProperties.CreditCardExpiryDateKey].ToString();
                    }

                    if (payment.PaymentMethod.PropertyBag.ContainsKey(CreditCardPaymentProperties.CreditCardBrandKey))
                    {
                        paymentMethodName = payment.PaymentMethod.PropertyBag[CreditCardPaymentProperties.CreditCardBrandKey].ToString();
                    }
                }
            }

            bool hasExpired = false;
            if (DateTime.TryParse(expiryDate, out DateTime expirationDate))
            {
                hasExpired = expirationDate < DateTime.UtcNow;
            }

            var paymentVm = new OrderSummaryPaymentViewModel
            {
                FirstName = payment.BillingAddress.FirstName,
                LastName = payment.BillingAddress.LastName,
                PaymentMethodName = paymentMethodName,
                CreditCardNumber = creditCartNumber,
                ExpiryDate = expiryDate,
                IsExpired = hasExpired
            };

            paymentVm.BillingAddress = CartViewModelFactory.GetAddressViewModel(payment.BillingAddress, cultureInfo);
            paymentVm.Amount = LocalizationProvider.FormatPrice(payment.Amount, cultureInfo);

            return paymentVm;
        }
    }
}