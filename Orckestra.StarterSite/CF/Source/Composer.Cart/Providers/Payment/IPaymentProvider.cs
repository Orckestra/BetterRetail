using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;

namespace Orckestra.Composer.Cart.Providers.Payment
{
    public interface IPaymentProvider
    {
        /// <summary>
        /// Name of the provider.
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// Type of Overture Payment Provider.
        /// </summary>
        string ProviderType { get; }

        /// <summary>
        /// Initializes the payment based on the PaymentProvider logic.
        /// </summary>
        /// <param name="cart">Cart.</param>
        /// <param name="param">Parameters used to make the request.</param>
        /// <returns></returns>
        Task<Overture.ServiceModel.Orders.Cart> InitializePaymentAsync(Overture.ServiceModel.Orders.Cart cart, InitializePaymentParam param);

        /// <summary>
        /// Determines if the payment information should be captured.
        /// </summary>
        /// <param name="payment">Payment information.</param>
        /// <returns>True if the payment should be captured. False otherwise.</returns>
        bool ShouldCapturePayment(Overture.ServiceModel.Orders.Payment payment);

        /// <summary>
        /// Gets the URL to capture the payment information.
        /// </summary>
        /// <param name="payment">Payment information.</param>
        /// <param name="cultureInfo">Culture of the request.</param>
        /// <returns>URL to capture the payment information.</returns>
        string GetCapturePaymentUrl(Overture.ServiceModel.Orders.Payment payment, CultureInfo cultureInfo);

        /// <summary>
        /// Augments the ViewModel with useful data.
        /// </summary>
        /// <param name="viewModel">The ViewModel to augment.</param>
        /// <param name="payment">The payment.</param>
        void AugmentViewModel(ActivePaymentViewModel viewModel, Overture.ServiceModel.Orders.Payment payment);

        OrderSummaryPaymentViewModel BuildOrderSummaryPaymentViewModel(Overture.ServiceModel.Orders.Payment payment, CultureInfo cultureInfo);
    }
}
