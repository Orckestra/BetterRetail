using System;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;

namespace Orckestra.Composer.Cart.Services.Order
{
    public interface IOrderHistoryViewService
    {
        /// <summary>
        /// Gets the OrderHistory ViewModel, containing a list of Orders.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<OrderHistoryViewModel> GetOrderHistoryViewModelAsync(GetCustomerOrdersParam param);

        /// <summary>
        /// Gets an OrderDetailViewModel, containing all information about an order and his shipments.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<OrderDetailViewModel> GetOrderDetailViewModelAsync(GetCustomerOrderParam param);

        /// <summary>
        /// Gets an OrderDetailViewModel for a guest customer, containing all information about an order and his shipments.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<OrderDetailViewModel> GetOrderDetailViewModelForGuestAsync(GetOrderForGuestParam param);

        /// <summary>
        /// Update Order Customer Id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Overture.ServiceModel.Orders.Order> UpdateOrderCustomerAsync(UpdateOrderCustomerParam param);

        /// <summary>
        /// Create edit order
        /// </summary>
        /// <param name="orderId">The Id of the order</param>
        /// <returns>View model of the editing order</returns>
        Task<EditingOrderViewModel> CreateEditingOrderViewModel(string orderNumber);

        /// <summary>
        /// Cancel edit order
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        Task CancelEditingOrderAsync(string orderNumber);

        /// <summary>
        /// Submit edit order
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        Task<CompleteCheckoutViewModel> SaveEditedOrderAsync(string orderNumber, string baseUrl);

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="orderId">The Id of the order</param>
        /// <returns>View model of the editing order</returns>
        Task<OrderFulfillmentState> CancelOrder(CancelOrderParam param);
    }
}