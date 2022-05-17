using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Grocery.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Services
{
    public class MyUsualsViewService : IMyUsualsViewService
    {
        private IMyUsualsSettings MyUsualsSettings { get; }
        protected IOrderRepository OrderRepository { get; private set; }


        public MyUsualsViewService(
            IMyUsualsSettings myUsualsSettings,
            IOrderRepository orderRepository)
        {
            MyUsualsSettings = myUsualsSettings ?? throw new ArgumentNullException(nameof(myUsualsSettings));
            OrderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));

        }

        public virtual async Task<string[]> GetMyUsualsProductSkusAsync(GetCustomerOrderedProductsParam param)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-MyUsualsSettings.TimeFrame);

            var response = await OrderRepository.GetCustomerOrderedProductsAsync(new GetCustomerOrderedProductsParam
            {
                ScopeId = param.ScopeId,
                CustomerId = param.CustomerId,
                StartDate = startDate,
                EndDate = endDate,
                MinimumOrderedNumberOfTimes = MyUsualsSettings.Frequency,
                MaximumItems = GroceryConfiguration.MaxOrderedProductsItems
            }).ConfigureAwait(false);

            var listSkus = response.OrderedProducts.Select(item => item.Sku).ToArray();

            return listSkus;
        }

    }
}
