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

        public virtual ActionResult OrderDetails(string token)
        {
            var orderToken = string.IsNullOrWhiteSpace(token) ? null : GuestOrderTokenizer.DecypherOrderToken(token);

            var findMyOrderUrl = OrderUrlProvider.GetFindMyOrderUrl(ComposerContext.CultureInfo);

            OrderDetailViewModel orderDetailViewModel = null;

            if (IsOrderTokenValid(orderToken))
            {
                orderDetailViewModel = OrderHistoryViewService.GetOrderDetailViewModelForGuestAsync(new GetOrderForGuestParam
                {
                    OrderNumber = orderToken.OrderNumber,
                    Email = orderToken.Email,
                    Scope = ComposerContext.Scope,
                    CultureInfo = ComposerContext.CultureInfo,
                    CountryCode = ComposerContext.CountryCode,
                    BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
                }).Result;
            }

            if (orderDetailViewModel != null) { return View("OrderDetailsContainer", orderDetailViewModel); }

            var findMyOrderUrlWithParams = UrlFormatter.AppendQueryString(findMyOrderUrl, new NameValueCollection
            {
                {"orderNotFound", "true"}
            });

            return Redirect(findMyOrderUrlWithParams);
        }

        private static bool IsOrderTokenValid(OrderToken orderToken)
        {
            var isValid = orderToken != null
                          && !string.IsNullOrWhiteSpace(orderToken.Email)
                          && !string.IsNullOrWhiteSpace(orderToken.OrderNumber);

            return isValid;
        }
    }
}