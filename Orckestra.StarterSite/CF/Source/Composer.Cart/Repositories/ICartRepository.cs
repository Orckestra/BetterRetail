using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;

namespace Orckestra.Composer.Cart.Repositories
{
    /// <summary>
    /// Abstraction for the repository that will be in charge of interacting with carts and lineitems.
    /// </summary>
    public interface ICartRepository
    {
        /// <summary>
        /// Add line item to cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> AddLineItemAsync(AddLineItemParam param);

        /// <summary>
        /// Adds a payment on the first shipment of the specified cart.
        /// </summary>
        /// <param name="param">Parameters used to add a payment to the cart.</param>
        /// <returns></returns>
        Task<ProcessedCart> AddPaymentAsync(AddPaymentParam param);

        /// <summary>
        /// Retrieves the list of carts belonging to a customer
        /// 
        /// param.IncludeChildScopes is optional
        /// A value indicating whether to include carts found in child scopes.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>List of Cart Summaries</returns>
        Task<List<CartSummary>> GetCartsByCustomerIdAsync(GetCartsByCustomerIdParam param);

        /// <summary>
        /// Retrieve a cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Processed Cart</returns>
        Task<ProcessedCart> GetCartAsync(GetCartParam param);

        /// <summary>
        /// Set the specified payment method as default for the user
        /// </summary>
        /// <param name="param"></param>
        /// <returns>Selected Payment Method</returns>
        Task<PaymentMethod> SetDefaultCustomerPaymentMethod(SetDefaultCustomerPaymentMethodParam param);

        /// <summary>
        /// Remove line item to the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated cart details</returns>
        Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param);

        /// <summary>
        /// Removes many line items from the cart at once.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> RemoveLineItemsAsync(RemoveLineItemsParam param);
        
        /// <summary>
        /// Update a lineItem in the cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated cart details</returns>
        Task<ProcessedCart> UpdateLineItemAsync(UpdateLineItemParam param);

        /// <summary>
        /// Adds a coupon to the Cart, then returns an instance of the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The full and updated cart details.</returns>
        Task<ProcessedCart> AddCouponAsync(CouponParam param);

        /// <summary>
        /// Removes the specified coupons for a specific cart instance.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task RemoveCouponsAsync(RemoveCouponsParam param);

        /// <summary>
        /// Update the Cart with new information
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> UpdateCartAsync(UpdateCartParam param);

        /// <summary>
        /// Update a shipment of a cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> UpdateShipmentAsync(UpdateShipmentParam param);

        /// <summary>
        /// Complete the checkout process
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The order</returns>
        Task<Overture.ServiceModel.Orders.Order> CompleteCheckoutAsync(CompleteCheckoutParam param);

        /// <summary>
        /// Get the recurring carts of a customer
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<List<ProcessedCart>> GetRecurringCartsAsync(GetRecurringOrderCartsViewModelParam param);

        /// <summary>
        /// Reschedule a recurring cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ListOfRecurringOrderLineItems> RescheduleRecurringCartAsync(RescheduleRecurringCartParam param);
        
        /// <summary>
        /// Remove a line item from a recurring cart
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<HttpWebResponse> RemoveRecurringCartLineItemAsync(RemoveRecurringCartLineItemParam param);
    }
}