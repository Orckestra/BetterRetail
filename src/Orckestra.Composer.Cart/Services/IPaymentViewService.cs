using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Payment Methods.
    /// </summary>
    public interface IPaymentViewService
    {
        /// <summary>
        /// Gets an enumeration of all available payment providers.
        /// </summary>
        /// <param name="param">Parameters used to retrieve the payment providers.</param>
        /// <returns></returns>
        Task<IEnumerable<PaymentProviderViewModel>> GetPaymentProvidersAsync(GetPaymentProvidersParam param);

        /// <summary>
        /// Get the Active Payment methods available for a cart.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethodViewModel</returns>
        Task<SingleCheckoutPaymentViewModel> GetSingleCheckoutPaymentAsync(GetPaymentMethodsParam param);

        /// <summary>
        /// Get the Active Payment methods available for a cart.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethodViewModel</returns>
        Task<CheckoutPaymentViewModel> GetPaymentMethodsAsync(GetPaymentMethodsParam param);

        /// <summary>
        /// Updates the payment method for a cart and initializes it.
        /// </summary>
        /// <param name="param">Parameters used to Update the payment method of the cart.</param>
        /// <returns>ViewModel representing the initialized Active Payment.</returns>
        Task<CheckoutPaymentViewModel> UpdatePaymentMethodAsync(UpdatePaymentMethodParam param);

        Task<ActivePaymentViewModel> UpdateActivePaymentMethodAsync(UpdatePaymentMethodParam param);

        /// <summary>
        /// Get the associated active payment for the specified cart
        /// </summary>
        /// <param name="param">Parameters used to get the payment for the cart</param>
        /// <returns>ViewModel representing the active payment for the specified cart</returns>
        Task<ActivePaymentViewModel> GetActivePayment(GetActivePaymentParam param);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        Task RemovePaymentMethodAsync(RemovePaymentMethodParam param);

        /// <summary>
        /// Returns the Saved Credit card for a customer and providers
        /// To be used in recurring orders menu
        /// </summary>
        Task<CustomerPaymentMethodListViewModel> GetCustomerPaymentMethodListViewModelAsync(GetCustomerPaymentMethodListViewModelParam param);

        /// <summary>
        /// Updates the payment method for a recurring cart 
        /// </summary>
        /// <param name="updatePaymentMethodParam"></param>
        /// <returns></returns>
        Task<CartViewModel>UpdateRecurringOrderCartPaymentMethodAsync(UpdatePaymentMethodParam param, string baseUrl);
    }
}
