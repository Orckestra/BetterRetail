using Orckestra.Composer.Cart.ViewModels.Order;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Services.Order
{
    public interface IOrderService
    {
        Task<EditingOrderViewModel> CreateEditOrder(Guid orderId);
    }
}
