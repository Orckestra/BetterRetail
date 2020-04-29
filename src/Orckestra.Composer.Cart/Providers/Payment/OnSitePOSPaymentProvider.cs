using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services.Lookup;

namespace Orckestra.Composer.Cart.Providers.Payment
{
    // ReSharper disable once InconsistentNaming
    public class OnSitePOSPaymentProvider : IPaymentProvider
    {
        private const string PosProviderTypeName = "OnSitePOSPaymentProvider";

        protected virtual ILookupService LookupService { get; private set; }
        protected virtual ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected virtual ILocalizationProvider LocalizationProvider { get; private set; }

        public OnSitePOSPaymentProvider(ILookupService lookupService, ICartViewModelFactory cartViewModelFactory,
            ILocalizationProvider localizationProvider)
        {
            CartViewModelFactory = cartViewModelFactory ?? throw new ArgumentNullException(nameof(cartViewModelFactory));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Type of Overture Payment Provider.
        /// </summary>
        public string ProviderType { get { return PosProviderTypeName; } }

        /// <summary>
        /// Initializes the payment based on the PaymentProvider logic.
        /// </summary>
        /// <param name="cart">Cart.</param>
        /// <param name="param">Parameters used to make the request.</param>
        /// <returns></returns>
        public Task<Overture.ServiceModel.Orders.Cart> InitializePaymentAsync(Overture.ServiceModel.Orders.Cart cart, InitializePaymentParam param)
        {
            return Task.FromResult(cart);
        }

        /// <summary>
        /// Determines if the payment information should be captured.
        /// </summary>
        /// <param name="payment">Payment information.</param>
        /// <returns>True if the payment should be captured. False otherwise.</returns>
        public virtual bool ShouldCapturePayment(Overture.ServiceModel.Orders.Payment payment)
        {
            return false;
        }

        /// <summary>
        /// Gets the URL to capture the payment information.
        /// </summary>
        /// <param name="payment">Payment information.</param>
        /// <param name="cultureInfo">Culture of the request.</param>
        /// <returns>URL to capture the payment information.</returns>
        public virtual string GetCapturePaymentUrl(Overture.ServiceModel.Orders.Payment payment, CultureInfo cultureInfo)
        {
            return null;
        }

        /// <summary>
        /// Augments the ViewModel with useful data.
        /// </summary>
        /// <param name="viewModel">The ViewModel to augment.</param>
        /// <param name="payment">The payment.</param>
        public virtual void AugmentViewModel(ActivePaymentViewModel viewModel, Overture.ServiceModel.Orders.Payment payment)
        {
            
        }

        public virtual OrderSummaryPaymentViewModel BuildOrderSummaryPaymentViewModel(Overture.ServiceModel.Orders.Payment payment, CultureInfo cultureInfo)
        {
            var methodDisplayNames = LookupService.GetLookupDisplayNamesAsync(new GetLookupDisplayNamesParam
            {
                CultureInfo = cultureInfo,
                LookupType = LookupType.Order,
                LookupName = "PaymentMethodType",
            }).Result;

            var paymentMethodDisplayName = methodDisplayNames.FirstOrDefault(x => x.Key == payment.PaymentMethod.Type.ToString()).Value;

            var paymentVm = new OrderSummaryPaymentViewModel
            {
                FirstName = payment.BillingAddress?.FirstName,
                LastName = payment.BillingAddress?.LastName,
                PaymentMethodName = paymentMethodDisplayName,
            };

            paymentVm.BillingAddress = CartViewModelFactory.GetAddressViewModel(payment.BillingAddress, cultureInfo);
            paymentVm.Amount = LocalizationProvider.FormatPrice((decimal)payment.Amount, cultureInfo);

            return paymentVm;
        }
    }
}
