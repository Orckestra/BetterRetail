using System;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class WishListController : ApiController
    {
        protected IWishListViewService WishListViewService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public WishListController(
            IWishListViewService wishListViewService,
            IComposerContext composerContext)
        {
            WishListViewService = wishListViewService ?? throw new ArgumentNullException(nameof(wishListViewService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        /// <summary>
        /// Get the wishlist for the current customer
        /// </summary>
        /// <returns>A Json representation of wishlist</returns>
        [HttpGet]
        [ActionName("getwishlist")]
        public virtual async Task<IHttpActionResult> GetWishList()
        {
            var viewModel = await WishListViewService.GetWishListViewModelAsync(new GetCartParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.WishlistCartName,
                ExecuteWorkflow = CartConfiguration.WishListExecuteWorkflow,
                WorkflowToExecute = CartConfiguration.WishListWorkflowToExecute,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                //WebsiteId = SiteConfiguration.GetWebsiteId()
            });

            return Ok(viewModel);
        }

        /// <summary>
        /// Get the wishlist for the current customer
        /// </summary>
        /// <returns>A LightWeight Json representation of wishlist( total items count, prodcut/variant ids</returns>
        [HttpGet]
        [ActionName("getwishlistsummary")]
        public virtual async Task<IHttpActionResult> GetWishListLight()
        {
            var viewModel = await WishListViewService.GetWishListSummaryViewModelAsync(new GetCartParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.WishlistCartName,
                ExecuteWorkflow = CartConfiguration.WishListExecuteWorkflow,
                WorkflowToExecute = CartConfiguration.WishListWorkflowToExecute,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(viewModel);
        }

        /// <summary>
        /// Add line item to the wishlist.
        /// WishList will be created if needed
        /// </summary>
        /// <param name="request">add args</param>
        /// <returns>A Json representation of the updated wishlist</returns>
        [HttpPost]
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> AddLineItem(AddLineItemViewModel request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            if (!ComposerContext.IsAuthenticated) { return BadRequest("Authorization required"); }

            var vm = await WishListViewService.AddLineItemAsync(new AddLineItemParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.WishlistCartName,
                ProductId = request.ProductId,
                VariantId = request.VariantId,
                Quantity = request.Quantity.GetValueOrDefault(),
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                RecurringOrderFrequencyName = request.RecurringOrderFrequencyName,
                RecurringOrderProgramName = request.RecurringOrderProgramName
            });

            return Ok(vm);
        }

        /// <summary>
        /// Remove line item from the wishlist.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A Json representation of the updated wishlist</returns>
        [HttpDelete]
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> RemoveLineItem(RemoveLineItemViewModel request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            if (!ComposerContext.IsAuthenticated) { return BadRequest("Authorization required"); }

            var vm = await WishListViewService.RemoveLineItemAsync(new RemoveLineItemParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                LineItemId = new Guid(request.LineItemId),
                CartName = CartConfiguration.WishlistCartName,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(vm);
        }
    }
}