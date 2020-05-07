using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.Utils;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class OrderBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IOrderUrlProvider OrderUrlProvider { get; private set; }
        protected IOrderHistoryViewService OrderHistoryViewService { get; private set; }

        protected OrderBaseController(
            IComposerContext composerContext,
            IOrderUrlProvider orderUrlProvider,
            IOrderHistoryViewService orderHistoryViewService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            OrderUrlProvider = orderUrlProvider ?? throw new ArgumentNullException(nameof(orderUrlProvider));
            OrderHistoryViewService = orderHistoryViewService ?? throw new ArgumentNullException(nameof(orderHistoryViewService));
        }

        public virtual ActionResult FindMyOrder(bool? orderNotFound)
        {
            var findMyOrderViewModel = new FindMyOrderViewModel
            {
                OrderNotFound = orderNotFound.GetValueOrDefault(false),
            };

            return View("FindMyOrderContainer", findMyOrderViewModel);
        }

    }
}