using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class PaymentController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IPaymentViewService PaymentViewService { get; private set; }
        protected IImageViewService ImageService { get; private set; }

        public PaymentController(IComposerContext composerContext, IPaymentViewService paymentViewService, IImageViewService imageService)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (imageService == null) { throw new ArgumentNullException("imageService"); }
            if (paymentViewService == null) { throw new ArgumentNullException(nameof(paymentViewService)); }

            ComposerContext = composerContext;
            PaymentViewService = paymentViewService;
            ImageService = imageService;
        }

        /// <summary>
        /// Get the Payment methods available for the current cart for a specific Payment Provider.
        /// </summary>
        /// <returns>A Json representation of the Payments methods</returns>
        [HttpPost]
        [ActionName("paymentmethods")]
        [ValidateModelState]
        public async Task<IHttpActionResult> GetPaymentMethods(GetPaymentMethodsViewModel request)
        {
            if (request == null) { return BadRequest("Request cannot be null."); }

            var trustImageVm = ImageService.GetCheckoutTrustImageViewModel(ComposerContext.CultureInfo);

            var vm = await PaymentViewService.GetPaymentMethodsAsync(new GetPaymentMethodsParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                ProviderNames = request.Providers.ToList(),
                CartName = CartConfiguration.ShoppingCartName,
                CustomerId = ComposerContext.CustomerId,
                IsAuthenticated = ComposerContext.IsAuthenticated
            });

            if (vm != null && vm.ActivePaymentViewModel != null)
            {
                vm.ActivePaymentViewModel.CreditCardTrustImage = trustImageVm;
            }

            return Ok(vm);
        }

        [HttpGet]
        [ActionName("activepayment")]
        public async Task<IHttpActionResult> GetActivePayment()
        {
            var vm = await PaymentViewService.GetActivePayment(new GetActivePaymentParam
            {
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                CartName = CartConfiguration.ShoppingCartName,
                Scope = ComposerContext.Scope,
                IsAuthenticated = ComposerContext.IsAuthenticated
            });

            if(vm != null)
                vm.CreditCardTrustImage = ImageService.GetCheckoutTrustImageViewModel(ComposerContext.CultureInfo);

            return Ok(vm);
        }

        [HttpDelete]
        [ActionName("removemethod")]
        public virtual async Task<IHttpActionResult> RemovePaymentMethod(RemovePaymentMethodViewModel request)
        {
            await PaymentViewService.RemovePaymentMethodAsync(new RemovePaymentMethodParam
            {
                PaymentMethodId = request.PaymentMethodId,
                CustomerId = ComposerContext.CustomerId,
                PaymentProviderName = request.PaymentProviderName,
                ScopeId = ComposerContext.Scope,
                CartName = CartConfiguration.ShoppingCartName
            }).ConfigureAwait(false);

            // Need to return at least a string otherwise jQuery ajax client 
            // will fail since it's expected valid json and void is not valid
            return Ok("OK");
        }

        [HttpPut]
        [ActionName("paymentMethod")]
        [ValidateModelState]
        public async Task<IHttpActionResult> UpdatePaymentMethod(UpdatePaymentMethodViewModel request)
        {
            if (request == null) { return BadRequest("Request cannot be null."); }

            var trustImageVm = ImageService.GetCheckoutTrustImageViewModel(ComposerContext.CultureInfo);

            var vm = await PaymentViewService.UpdatePaymentMethodAsync(new UpdatePaymentMethodParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                PaymentId = request.PaymentId.GetValueOrDefault(),
                Scope = ComposerContext.Scope,
                PaymentMethodId = request.PaymentMethodId.GetValueOrDefault(),
                PaymentProviderName = request.PaymentProviderName,
                PaymentType = request.PaymentType,
                ProviderNames = request.Providers.ToList(),
                IsAuthenticated = ComposerContext.IsAuthenticated
            }).ConfigureAwait(false);

            vm.ActivePaymentViewModel.CreditCardTrustImage = trustImageVm;

            return Ok(vm);
        }
    }
}
