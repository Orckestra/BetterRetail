using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
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
        protected IRecurringScheduleUrlProvider RecurringScheduleUrlProvider { get; private set; }
        protected IRecurringCartUrlProvider RecurringCartUrlProvider { get; private set; }

        public PaymentController(IComposerContext composerContext, IPaymentViewService paymentViewService, IImageViewService imageService,
            IRecurringScheduleUrlProvider recurringScheduleUrlProvider, IRecurringCartUrlProvider recurringCartUrlProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            PaymentViewService = paymentViewService ?? throw new ArgumentNullException(nameof(paymentViewService));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            RecurringScheduleUrlProvider = recurringScheduleUrlProvider ?? throw new ArgumentNullException(nameof(recurringScheduleUrlProvider));
            RecurringCartUrlProvider = recurringCartUrlProvider ?? throw new ArgumentNullException(nameof(recurringCartUrlProvider));
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

            if (vm?.ActivePaymentViewModel != null)
            {
                vm.ActivePaymentViewModel.CreditCardTrustImage = trustImageVm;
            }

            return Ok(vm);
        }

        /// <summary>
        /// Get the Payment providers and methods available for the current cart.
        /// </summary>
        /// <returns>A Json representation of the Payments methods</returns>
        [HttpGet]
        [ActionName("checkoutpayment")]
        [ValidateModelState]
        public async Task<IHttpActionResult> GetCheckoutPayment()
        {
            var vm = await PaymentViewService.GetSingleCheckoutPaymentAsync(new GetPaymentMethodsParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CartName = CartConfiguration.ShoppingCartName,
                CustomerId = ComposerContext.CustomerId,
                IsAuthenticated = ComposerContext.IsAuthenticated
            });

            vm.CreditCardTrustImage = ImageService.GetCheckoutTrustImageViewModel(ComposerContext.CultureInfo);

            return Ok(vm);
        }

        [HttpPut]
        [ActionName("checkoutpayment")]
        [ValidateModelState]
        public async Task<IHttpActionResult> UpdateCheckoutPayment(UpdatePaymentMethodViewModel request)
        {
            if (request == null) { return BadRequest("Request cannot be null."); }

            var param = new UpdatePaymentMethodParam
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
            };

            var vm = await PaymentViewService.UpdateActivePaymentMethodAsync(param);

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

            if (vm != null)
            {
                vm.CreditCardTrustImage = ImageService.GetCheckoutTrustImageViewModel(ComposerContext.CultureInfo);
            }  

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

        [HttpDelete]
        [ActionName("recurringcartremovemethod")]
        public virtual async Task<IHttpActionResult> RecurringCartRemovePaymentMethod(RecurringCartRemovePaymentMethodViewModel request)
        {
            await PaymentViewService.RemovePaymentMethodAsync(new RemovePaymentMethodParam
            {
                PaymentMethodId = request.PaymentMethodId,
                CustomerId = ComposerContext.CustomerId,
                PaymentProviderName = request.PaymentProviderName,
                ScopeId = ComposerContext.Scope,
                CartName = request.CartName
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

        [HttpPost]
        [ActionName("recurringorderstemplatespaymentmethods")]
        public virtual async Task<IHttpActionResult> GetRecurringOrderTemplatesPaymentMethodsAsync(GetRecurringOrderTemplatesPaymentMethodsRequest request)
        {
            var recurringOrderScheduleUrl = RecurringScheduleUrlProvider.GetRecurringScheduleDetailsUrl(new GetRecurringScheduleDetailsUrlParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                RecurringScheduleId = request.Id
            });

            //TODO : MyWalletPRovider when ready

            //var addAddressUrl = MyWalletPRovider.GetAddWAlletUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo, ReturnUrl = recurringOrderScheduleUrl });
            //var editAddressBaseUrl = MyWalletPRovider.GetUpdateWAlletBaseUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo });

            var param = new GetPaymentProvidersParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo
            };
            var providers = await PaymentViewService.GetPaymentProvidersAsync(param).ConfigureAwait(false);

            var results = await PaymentViewService.GetCustomerPaymentMethodListViewModelAsync(new GetCustomerPaymentMethodListViewModelParam
            {
                CustomerId = ComposerContext.CustomerId,
                ScopeId = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                ProviderNames = providers.Select(p => p.ProviderName).ToList(),
                //walletUrls
            }).ConfigureAwait(false);

            return Ok(results);
        }

        [HttpPost]
        [ActionName("recurringcartspaymentmethods")]
        public virtual async Task<IHttpActionResult> GetRecurringCartsPaymentMethodsAsync(GetRecurringCartsPaymentMethodsRequest request)
        {
            var recurringCartUrl = RecurringCartUrlProvider.GetRecurringCartDetailsUrl(new GetRecurringCartDetailsUrlParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                RecurringCartName = request.CartName
            });

            //TODO : MyWalletPRovider when ready

            //var addAddressUrl = MyWalletPRovider.GetAddWAlletUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo, ReturnUrl = recurringOrderScheduleUrl });
            //var editAddressBaseUrl = MyWalletPRovider.GetUpdateWAlletBaseUrl(new GetMyAccountUrlParam { CultureInfo = ComposerContext.CultureInfo });

            //Will need to change Viewmodel returned

            var providers = await PaymentViewService.GetPaymentProvidersAsync(new GetPaymentProvidersParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
            }).ConfigureAwait(false);

            var results = await PaymentViewService.GetPaymentMethodsAsync(new GetPaymentMethodsParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CartName = request.CartName,
                CustomerId = ComposerContext.CustomerId,
                ProviderNames = providers.Select(p => p.ProviderName).ToList(),
                IsAuthenticated = ComposerContext.IsAuthenticated
            });

            return Ok(results);
        }
    }
}