using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Providers.Order
{
    public interface IEditingOrderProvider
    {
        Task<bool> IsOrderEditable(Overture.ServiceModel.Orders.Order order);

        bool IsCurrentEditingOrder(Overture.ServiceModel.Orders.Order order);

        bool IsEditMode();

        string GetCurrentEditingCartName();

        Task<ProcessedCart> StartEditOrderModeAsync(Overture.ServiceModel.Orders.Order order);
    }
}
