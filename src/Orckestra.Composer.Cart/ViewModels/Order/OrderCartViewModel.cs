using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public class OrderCartViewModel {
        public OrderItem OrderItem { get; set; }
        public Overture.ServiceModel.Orders.Cart Cart { get; set; }
    }
}
