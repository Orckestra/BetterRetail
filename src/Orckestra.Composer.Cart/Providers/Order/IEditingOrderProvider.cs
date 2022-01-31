using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Providers.Order
{
    public interface IEditingOrderProvider
    {
        Task<bool> IsOrderEditable(Overture.ServiceModel.Orders.Order order);

        bool IsCurrentEditingOrder(Overture.ServiceModel.Orders.Order order);

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
    }
}
