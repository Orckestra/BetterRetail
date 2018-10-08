using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels.Order;

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
    }
}