using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class RecurringCartController : ApiController    
    {
        protected IRecurringOrderCartsViewService RecurringOrderCartsService { get; }
        protected IComposerContext ComposerContext { get; }
        protected IPaymentViewService PaymentViewService { get; }
        protected IRecurringOrderTemplatesViewService RecurringOrderTemplatesService { get; }
        protected IShippingMethodViewService ShippingMethodViewService { get; }
        protected ICartService CartService { get; }

        public RecurringCartController(
            IRecurringOrderCartsViewService recurringOrderCarstService,
            IComposerContext composerContext,
            IPaymentViewService paymentViewService,
            IRecurringOrderTemplatesViewService recurringOrderTemplatesService,
            IShippingMethodViewService shippingMethodViewService,
            ICartService cartService)
        {
            if (recurringOrderCarstService == null) throw new ArgumentNullException(nameof(recurringOrderCarstService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(recurringOrderCarstService)));
            if (composerContext == null) throw new ArgumentNullException(nameof(composerContext), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(composerContext)));
            if (paymentViewService == null) throw new ArgumentNullException(nameof(paymentViewService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(paymentViewService)));
            if (recurringOrderTemplatesService == null) throw new ArgumentNullException(nameof(recurringOrderTemplatesService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(recurringOrderTemplatesService)));
            if (shippingMethodViewService == null) throw new ArgumentNullException(nameof(shippingMethodViewService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(shippingMethodViewService)));
            if (cartService == null) throw new ArgumentNullException(nameof(shippingMethodViewService), ArgumentNullMessageFormatter.FormatErrorMessage(nameof(cartService)));

            RecurringOrderCartsService = recurringOrderCarstService;
            ComposerContext = composerContext;
            PaymentViewService = paymentViewService;
            RecurringOrderTemplatesService = recurringOrderTemplatesService;
            ShippingMethodViewService = shippingMethodViewService;
            CartService = cartService;
        }

        [HttpGet]
        [ActionName("getrecurringordercarts")]
        public virtual async Task<IHttpActionResult> GeRecurringOrderCartsByUser()
        {
            //This call manages products/variants that have been deleted in templates.
            //When cleaning those templates, it should clean the carts too.
            //In most cases, generating the templates is not a big load, if it's a problem, create a new call in 
            //RecurringOrderTemplateViewModelFactory to only check templates are fine and clean up if not.
            var templatesVm = await RecurringOrderTemplatesService.GetRecurringOrderTemplatesViewModelAsync(new GetRecurringOrderTemplatesParam
            {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            var results = await RecurringOrderCartsService.GetRecurringOrderCartListViewModelAsync(new GetRecurringOrderCartsViewModelParam
            {
                Scope = ComposerContext.Scope,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpGet]
        [ActionName("upcoming-orders")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetPastOrders()
        {
            var viewModel = await RecurringOrderCartsService.GetLightRecurringOrderCartListViewModelAsync(new GetLightRecurringOrderCartListViewModelParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(viewModel);
        }

        [HttpPut]
        [ActionName("address")]
        public virtual async Task<IHttpActionResult> UpdateRecurringOrderCartAddress([FromBody]UpdateRecurringOrderCartAddressRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (request.ShippingAddressId == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var param = new UpdateRecurringOrderCartShippingAddressParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                ScopeId = ComposerContext.Scope,
                CartName = request.cartName,
                CustomerId = ComposerContext.CustomerId,
                ShippingAddressId = request.ShippingAddressId.ToGuid(),
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                UseSameForShippingAndBilling = request.UseSameForShippingAndBilling
            };

            var results = await RecurringOrderCartsService.UpdateRecurringOrderCartShippingAddressAsync(param).ConfigureAwait(false);

            return Ok(results);
        }
        
        [HttpPut]
        [ActionName("shippingmethod")]
        public virtual async Task<IHttpActionResult> UpdateRecurringCartShippingMethod([FromBody]UpdateRecurringCartShippingMethodRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (request.ShippingProviderId == null) { return BadRequest("Missing Request Body"); }
            if (request.ShippingMethodName == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            

            var results = await ShippingMethodViewService.UpdateRecurringOrderCartShippingMethodAsync(new UpdateRecurringOrderCartShippingMethodParam()
            {
                CartName = request.CartName,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                ShippingMethodName = request.ShippingMethodName,
                ShippingProviderId = request.ShippingProviderId
            });

            return Ok(results);
        }

        [HttpPut]
        [ActionName("reschedule")]
        public async Task<IHttpActionResult> UpdateRecurringCartNextOccurence([FromBody]UpdateRecurringCartNextOccurenceRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await RecurringOrderCartsService.UpdateRecurringOrderCartNextOccurenceAsync(new UpdateRecurringOrderCartNextOccurenceParam()
            {
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                CartName = request.CartName,
                NextOccurence = request.NextOccurence,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),

            }).ConfigureAwait(false);

            return Ok(vm);
        }

        [HttpPost]
        [ActionName("getrecurringcart")]
        public async Task<IHttpActionResult> GetRecurringCart([FromBody]GetRecurringCartRequest request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await RecurringOrderCartsService.GetRecurringOrderCartViewModelAsync(new GetRecurringOrderCartViewModelParam
            {
                CartName = request.Name,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwait(false); ;

            return Ok(vm);
        }

        /// <summary>
        /// Update the line item in the recurring cart.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> UpdateLineItem(UpdateRecurringCartLineItemViewModel request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            
            var vm = await CartService.UpdateLineItemAsync(new UpdateLineItemParam
            {
                ScopeId = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                LineItemId = new Guid(request.LineItemId),
                CartName = request.CartName,
                Quantity = request.Quantity.GetValueOrDefault(),
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                RecurringOrderFrequencyName = request.RecurringOrderFrequencyName,
                RecurringOrderProgramName = request.RecurringOrderProgramName
            });

            return Ok(vm);
        }

        /// <summary>
        /// Remove line item to the cart.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A Json representation of the updated cart state</returns>
        [HttpDelete]
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> RemoveLineItem(RemoveRecurringCartLineItemViewModel request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await RecurringOrderCartsService.RemoveLineItemAsync(new RemoveRecurringCartLineItemParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                LineItemId = new Guid(request.LineItemId),
                CartName = request.CartName,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            }).ConfigureAwaitWithCulture(false);

            return Ok(vm);
        }
    }
}
