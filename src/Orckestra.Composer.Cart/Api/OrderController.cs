using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.Utils;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [Authorize]
    [JQueryOnlyFilter]
    public class OrderController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IOrderHistoryViewService OrderHistoryViewService { get; private set; }
        protected IOrderUrlProvider OrderUrlProvider { get; private set; }

        public OrderController(
            IComposerContext composerContext,
            IOrderHistoryViewService orderHistoryViewService,
            IOrderUrlProvider orderUrlProvider)
        {
            OrderHistoryViewService = orderHistoryViewService ?? throw new ArgumentNullException(nameof(orderHistoryViewService));
            OrderUrlProvider = orderUrlProvider;
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        [HttpPost]
        [ActionName("current-orders")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetCurrentOrders(GetOrdersParam param)
        {
            if (param == null) { return BadRequest("No request found."); }

            var viewModel = await OrderHistoryViewService.GetOrderHistoryViewModelAsync(new GetCustomerOrdersParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                Page = param.Page,
                OrderTense = OrderTense.CurrentOrders,
                //WebsiteId = SiteConfiguration.GetWebsiteId()
            });

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("past-orders")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetPastOrders(GetOrdersParam param)
        {
            if (param == null) { return BadRequest("No request found."); }

            var viewModel = await OrderHistoryViewService.GetOrderHistoryViewModelAsync(new GetCustomerOrdersParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                Page = param.Page,
                OrderTense = OrderTense.PastOrders
            });

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("url")]
        [ValidateModelState]
        [AllowAnonymous]
        public virtual async Task<IHttpActionResult> GetGuestOrderDetailsUrl(GetGuestOrderViewModel request)
        {
            if (request == null) { return BadRequest("No request body found."); }

            var orderDetailUrl = OrderUrlProvider.GetGuestOrderDetailsUrl(ComposerContext.CultureInfo);

            var orderDetailViewModel = await OrderHistoryViewService.GetOrderDetailViewModelForGuestAsync(new GetOrderForGuestParam
            {
                OrderNumber = request.OrderNumber,
                Email = request.Email,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CountryCode = ComposerContext.CountryCode,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            if (orderDetailViewModel == null) { return NotFound(); }

            var token = GuestOrderTokenizer.GenerateOrderToken(new OrderToken
            {
                Email = request.Email,
                OrderNumber = request.OrderNumber
            });

            var url = UrlFormatter.AppendQueryString(orderDetailUrl, new NameValueCollection
            {
                {"token", token }
            });

            var vm = new GuestOrderDetailsViewModel
            {
                Url = url
            };

            return Ok(vm);
        }
    }
}