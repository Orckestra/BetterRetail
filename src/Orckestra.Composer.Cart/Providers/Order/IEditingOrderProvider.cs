using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;
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
        /// Is Order can be canceled
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> CanCancel(Overture.ServiceModel.Orders.Order order);

        /// <summary>
        /// Is Order in pending cancel status
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<bool> PendingCancel(Overture.ServiceModel.Orders.Order order);

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task CancelOrder(Overture.ServiceModel.Orders.Order order, string scope);

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
