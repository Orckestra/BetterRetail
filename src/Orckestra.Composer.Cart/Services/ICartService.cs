using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Cart.
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Add line item to cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CartViewModel> AddLineItemAsync(AddLineItemParam param);

        /// <summary>
        /// Retrieve a cart and create CartViewModel
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Lightweight CartViewModel</returns>
        Task<CartViewModel> GetCartViewModelAsync(GetCartParam param);

        /// <summary>
        /// Create CartViewModel
        /// </summary>
        /// <returns>CartViewModel</returns>
        Task<CartViewModel> CreateCartViewModelAsync(CreateCartViewModelParam param);

        /// <summary>
        /// Remove line item to the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Lightweight CartViewModel</returns>
        Task<CartViewModel> RemoveLineItemAsync(RemoveLineItemParam param);

        /// <summary>
        /// Update a line item in the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Lightweight CartViewModel</returns>
        Task<CartViewModel> UpdateLineItemAsync(UpdateLineItemParam param);

        /// <summary>
        /// Update the cart
        /// </summary>
        /// <returns>The Lightweight CartViewModel</returns>
        Task<CartViewModel> UpdateCartAsync(UpdateCartViewModelParam param);

        /// <summary>
        /// Removes all invalid line items in the cart and returns view model
        /// </summary>
        /// <param name="param">Parameters used to clean the cart of the invalid line items.</param>
        /// <returns>Cart view model</returns>
        Task<CartViewModel> RemoveInvalidLineItemsAsync(RemoveInvalidLineItemsParam param);

        /// <summary>
        /// Execute removal part of invalid line items from the cart
        /// </summary>
        /// <param name="param">Parameters </param>
        /// <returns>Processed cart</returns>
        Task<ProcessedCart> ProcessInvalidLineItemsRemovalAsync(RemoveInvalidLineItemsParam param);

        /// <summary>
        /// Update the shipping address postal code in the cart
        /// </summary>
        /// <returns>The Lightweight CartViewModel</returns>
        Task<CartViewModel> UpdateShippingAddressPostalCodeAsync(UpdateShippingAddressPostalCodeParam param);

        /// <summary>
        /// Update the billing address postal code in the cart
        /// </summary>
        /// <returns>The Lightweight CartViewModel</returns>
        Task<CartViewModel> UpdateBillingAddressPostalCodeAsync(UpdateBillingAddressPostalCodeParam param);

        Task<IPaymentMethodViewModel> SetDefaultCustomerPaymentMethod(SetDefaultCustomerPaymentMethodParam param);
    }
}
