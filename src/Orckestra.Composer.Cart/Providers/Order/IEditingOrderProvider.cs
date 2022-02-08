using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;

namespace Orckestra.Composer.Cart.Providers.Order
{
    public interface IEditingOrderProvider
    {
        /// <summary>
        /// Is Order can be edited
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> CanEdit(Overture.ServiceModel.Orders.Order order);

        /// <summary>
        /// Is Order being edited now
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool IsBeingEdited(Overture.ServiceModel.Orders.Order order);

        /// <summary>
        /// Get Order Cancellation Status
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<CancellationStatus> GetCancellationStatus(Overture.ServiceModel.Orders.Order order);
        
        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task CancelOrder(Overture.ServiceModel.Orders.Order order);

        /// <summary>
        /// Is Edit mode right now on the website
        /// </summary>
        /// <returns></returns>
        bool IsEditMode();

        void ClearEditMode();

        string GetCurrentEditingCartName();

        /// <summary>
        /// Create Cart Order Draft and set edit mode
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<ProcessedCart> StartEditOrderModeAsync(Overture.ServiceModel.Orders.Order order);

        /// <summary>
        /// Delete Cart Order Draft and clear edit mode
        /// </summary>
        /// <param name="order"></param>
        Task CancelEditOrderAsync(Overture.ServiceModel.Orders.Order order);

        /// <summary>
        /// Submit Cart Order Draft to the order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<Overture.ServiceModel.Orders.Order> SaveEditedOrderAsync(Overture.ServiceModel.Orders.Order order);
    }
}
