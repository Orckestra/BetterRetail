using Orckestra.Composer.Cart.Repositories.Order;
using System;
using System.Threading.Tasks;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Constants.General;
using Orckestra.Overture;
using Orckestra.Composer.Services;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Cart.Services.Order
{
    public class OrderService : IOrderService
    {
        public OrderService(
            IOrderRepository orderRepository, 
            IOvertureClient overtureClient, 
            IComposerContext composerContext, 
            ICartUrlProvider cartUrlProvider)
        {
            OrderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CartUrlProvider = cartUrlProvider ?? throw new ArgumentNullException(nameof(cartUrlProvider));
        }

        protected virtual IOrderRepository OrderRepository { get; private set; }
        protected virtual IOvertureClient OvertureClient { get; private set; }
        public IComposerContext ComposerContext { get; }
        public ICartUrlProvider CartUrlProvider { get; private set; }

        public async Task<EditingOrderViewModel> CreateEditOrder(Guid orderId)
        {
            if (orderId == Guid.Empty) throw new ArgumentException(GetMessageOfEmpty(nameof(orderId)));

            var getOrderByIdParam = new GetOrderByIdParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                OrderId = orderId,
                Scope = GlobalScopeName,
                IncludeShipment = true  
            };

            var order = await OrderRepository.GetOrderByIdAsync(getOrderByIdParam).ConfigureAwait(false);

            if (order?.Cart?.Shipments == null || order.Cart.Shipments.Count == 0)
                throw new InvalidOperationException("Cannot edit this order");

            var createOrderDraftParam = new CreateCartOrderDraftParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                OrderId = orderId,
                Scope = order.ScopeId,
                CustomerId = Guid.Parse(order.CustomerId)
            };

            var editCart = await OrderRepository.CreateCartOrderDraft(createOrderDraftParam).ConfigureAwait(false);
            if (editCart == null)
            {
                throw new InvalidOperationException("Expected draft cart, but received null.");
            }

            var viewModel = new EditingOrderViewModel
            {
                Scope = editCart.ScopeId,
                OrderId = orderId,
                CartUrl = CartUrlProvider.GetCartUrl(new BaseUrlParameter
                {
                    CultureInfo = ComposerContext.CultureInfo
                })
            };

            return viewModel;
        }
    }
}
