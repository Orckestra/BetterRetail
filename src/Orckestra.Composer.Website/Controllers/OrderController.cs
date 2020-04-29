using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class OrderController : OrderBaseController
    {
        public OrderController(IComposerContext composerContext, IOrderUrlProvider orderUrlProvider, IOrderHistoryViewService orderHistoryViewService) 
            : base(composerContext, orderUrlProvider, orderHistoryViewService) { }
    }
}