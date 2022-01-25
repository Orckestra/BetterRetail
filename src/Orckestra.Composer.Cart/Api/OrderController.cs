using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.Utils;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using static Orckestra.Composer.Constants.General;
using static Orckestra.Composer.Cart.Extensions.CartExtensions;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [Authorize]
    [JQueryOnlyFilter]
    public class OrderController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IOrderRepository OrderRepository { get; private set; }
        protected IOrderHistoryViewService OrderHistoryViewService { get; private set; }
        protected IOrderUrlProvider OrderUrlProvider { get; private set; }
        protected ICartUrlProvider CartUrlProvider { get; private set; }


        public OrderController(
            IComposerContext composerContext,
            IOrderHistoryViewService orderHistoryViewService,
            IOrderUrlProvider orderUrlProvider,
            ICartUrlProvider cartUrlProvider
            )
        {
            OrderHistoryViewService = orderHistoryViewService ?? throw new ArgumentNullException(nameof(orderHistoryViewService));
            OrderUrlProvider = orderUrlProvider ?? throw new ArgumentNullException(nameof(orderUrlProvider));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CartUrlProvider = cartUrlProvider ?? throw new ArgumentNullException(nameof(cartUrlProvider));
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
        /// <param name="request">Parameters container</param>
        [HttpPost]
        [ActionName("edit-order")]
        public virtual async Task<IHttpActionResult> EditOrder(EditOrderParam request)
        {
            var editedCart = await OrderRepository.CreateEditOrder(GlobalScopeName, request.OrderId);

            ComposerContext.EditingOrderScope = editedCart.ScopeId;
            ComposerContext.EditingOrderNumber = request.OrderNumber;
            ComposerContext.EditingOrderId = request.OrderId;
            ComposerContext.EditingOrderUntil = editedCart.GetOrderEditableUntilDate();

            return Ok(GetEditingOrderViewModel());
        }

        /// <summary>
        /// Save edited order
        /// </summary>
        /// <param name="request">Parameters container</param>
        [HttpPost]
        [ActionName("save-edited-order")]
        public virtual async Task<IHttpActionResult> SaveEditedOrder()
        {
            if (!ComposerContext.IsEditingOrder) return Ok();

            if (DateTime.UtcNow > ComposerContext.EditingOrderUntil)
            {
                await OrderRepository.CancelEditOrder(ComposerContext.EditingOrderScope, ComposerContext.EditingOrderId).ConfigureAwait(false);
                ComposerContext.ClearEditingOrder();

                throw new InvalidOperationException("The order is no longer editable.");
            }

            await OrderRepository.SaveEditedOrder(ComposerContext.EditingOrderScope, ComposerContext.EditingOrderId).ConfigureAwait(false);

            var editingOrderNumber = ComposerContext.EditingOrderNumber;
            ComposerContext.ClearEditingOrder();

            var redirectUrl = CartUrlProvider.GetCheckoutConfirmationPageUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            });
            var vm = new { RedirectUrl = $"{redirectUrl}?id={editingOrderNumber}" };

            return Ok(vm);
        }

        /// <summary>
        /// Cancels editing of an order
        /// </summary>
        /// <param name="request">Parameters container</param>
        [HttpPost]
        [ActionName("cancel-edit-order")]
        public virtual async Task<IHttpActionResult> CancelEditOrder()
        {
            if (ComposerContext.IsEditingOrder)
            {
                await OrderRepository.CancelEditOrder(ComposerContext.EditingOrderScope, ComposerContext.EditingOrderId);
                ComposerContext.ClearEditingOrder();
            }

            var redirectUrl = OrderUrlProvider.GetOrderHistoryUrl(new GetOrderUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            });
            var vm = new { RedirectUrl = redirectUrl };

            return Ok(vm);
        }

        [HttpPost]
        [ActionName("get-edited-order")]
        public virtual IHttpActionResult GetEditOrder()
        {
            return Ok(GetEditingOrderViewModel());
        }

        private EditingOrderViewModel GetEditingOrderViewModel()
        {
            return new EditingOrderViewModel
            {
                Scope = ComposerContext.EditingOrderScope,
                OrderNumber = ComposerContext.EditingOrderNumber,
                CartUrl = CartUrlProvider.GetCartUrl(new BaseUrlParameter
                {
                    CultureInfo = ComposerContext.CultureInfo
                }),
                EditableUntil = ComposerContext.EditingOrderUntil
            };
        }
    }
}
