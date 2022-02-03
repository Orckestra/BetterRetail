using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.Utils;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Http;

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
            OrderUrlProvider = orderUrlProvider ?? throw new ArgumentNullException(nameof(orderUrlProvider));
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

        [HttpPost]
        [ActionName("attach-customer")]
        public virtual async Task<IHttpActionResult> AddOrderToCustomer(string id)
        {
            if (string.IsNullOrEmpty(id)) { return BadRequest("No order found."); }

            var viewModel = await OrderHistoryViewService.UpdateOrderCustomerAsync(new UpdateOrderCustomerParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                OrderNumber = id
            });

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("orderbynumber")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetOrderByNumber(GetOrderRequest param)
        {
            if (param == null) { return BadRequest("No request found."); }

            var viewModel = await OrderHistoryViewService.GetOrderDetailViewModelAsync(new GetCustomerOrderParam
            {
                OrderNumber = param.OrderNumber,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CountryCode = ComposerContext.CountryCode,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CustomerId = ComposerContext.CustomerId
            });

            return Ok(viewModel);
        }

        [HttpPost]
        [ActionName("guestorderbynumber")]
        [ValidateModelState]
        [AllowAnonymous]
        public virtual async Task<IHttpActionResult> GetGuestOrderByNumber(GetGuestOrderRequest param)
        {
            if (param == null) { return BadRequest("No request found."); }

            var viewModel = await OrderHistoryViewService.GetOrderDetailViewModelForGuestAsync(new GetOrderForGuestParam
            {
                OrderNumber = param.OrderNumber,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CountryCode = ComposerContext.CountryCode,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                Email = param.Email
            });

            return Ok(viewModel);
        }

        /// <summary>
        /// Set an order in edit mode
        /// </summary>
        /// <param name="param">Parameters container</param>
        [HttpPost]
        [ActionName("edit-order")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> EditOrder(EditOrderParam param)
        {
            if (param == null) return BadRequest($"{nameof(param)} cannot be empty");

            var vm = await OrderHistoryViewService.CreateEditingOrderViewModel(param.OrderNumber).ConfigureAwait(false);

            return Ok(vm);
        }


        /// <summary>
        /// Cancel edit mode for order
        /// </summary>
        /// <param name="param">Parameters container</param>
        [HttpPost]
        [ActionName("cancel-edit-order")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> CancelEditOrder(EditOrderParam param)
        {
            if (param == null) return BadRequest($"{nameof(param)} cannot be empty");

            await OrderHistoryViewService.CancelEditingOrderAsync(param.OrderNumber).ConfigureAwait(false);

            return Ok(true);
        }

        /// <summary>
        /// Set an order status to cancel 
        /// </summary>
        /// <param name="param">Parameters container</param>
        [HttpPost]
        [ActionName("cancel-order")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> CancelOrder([FromBody]string orderNumber)
        {
            var param = new CancelOrderParam()
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                OrderNumber = orderNumber
            };

            var vm = await OrderHistoryViewService.CancelOrder(param).ConfigureAwait(false);

            return Ok(vm);
        }
    }
}
