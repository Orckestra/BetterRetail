using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Repositories
{
    public class SalesScopeOrderRepository : OrderRepository
    {
        protected IScopeProvider ScopeProvider { get; set; }

        public SalesScopeOrderRepository(IOvertureClient overtureClient,
            IFindOrdersRequestFactory findOrdersRequestFactory,
            IScopeProvider scopeProvider) : base(overtureClient, findOrdersRequestFactory)
        {
            ScopeProvider = scopeProvider;
        }

        public override Task<OrderQueryResult> GetCustomerOrdersAsync(GetCustomerOrdersParam param)
        {
            param.Scope = ScopeProvider.DefaultScope;
            return base.GetCustomerOrdersAsync(param);
        }

        public override Task<Order> GetOrderAsync(GetOrderParam param)
        {
            param.Scope = ScopeProvider.DefaultScope;
            return base.GetOrderAsync(param);
        }
    }
}