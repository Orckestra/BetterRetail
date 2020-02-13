using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public interface IFindOrdersRequestFactory
    {
        FindOrdersRequest Create(GetCustomerOrdersParam param);
    }
}