using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Requests;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class CartController : ApiController
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();
        protected ICartService CartService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ICouponViewService CouponViewService { get; private set; }
        protected ICheckoutService CheckoutService { get; private set; }
        protected IShippingMethodViewService ShippingMethodService { get; private set; }
        protected ICartUrlProvider CartUrlProvider { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        public CartController(
            ICartService cartService,
            ICheckoutService checkoutService,
            IComposerContext composerContext,
            ICouponViewService couponViewService,
            IShippingMethodViewService shippingMethodService,
            ICartUrlProvider cartUrlProvider,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            CartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CouponViewService = couponViewService ?? throw new ArgumentNullException(nameof(couponViewService));
            CheckoutService = checkoutService ?? throw new ArgumentNullException(nameof(checkoutService));
            ShippingMethodService = shippingMethodService ?? throw new ArgumentNullException(nameof(shippingMethodService));
            CartUrlProvider = cartUrlProvider ?? throw new ArgumentNullException(nameof(cartUrlProvider));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
        }

        /// <summary>
        /// Get the shopping cart for the current customer
        /// </summary>
        /// <returns>A Json representation of cart state</returns>
        [HttpGet]
        [ActionName("getcart")]
        public virtual async Task<IHttpActionResult> GetCart()
        {
            var cartViewModel = await CartService.GetCartViewModelAsync(new GetCartParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
            });

            if (cartViewModel.OrderSummary != null)
            {
                try
                {
                    if (cartViewModel.IsCartEmpty)
                    {
                        cartViewModel.OrderSummary.CheckoutRedirectAction.RedirectUrl = GetCartUrl();
                    }
                    else
                    {
                        //If the cart contains recurring items and user is not authenticated, redirect to sign in
                        if (RecurringOrdersSettings.Enabled && cartViewModel.HasRecurringLineitems && !ComposerContext.IsAuthenticated)
                        {
                            cartViewModel.OrderSummary.CheckoutRedirectAction.RedirectUrl = GetCheckoutSignInUrl();
                        }
                    }
                }
                catch (ArgumentException e) {
                    Log.Error(e.ToString());
                };
            }

            return Ok(cartViewModel);
        }

        /// <summary>
        /// Update a Cart during the checkout process.
        /// </summary>
        /// <param name="updateCartRequest">The modifications to the cart</param>
        [HttpPost]
        [ActionName("updatecart")]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> UpdateCart(UpdateCartRequest updateCartRequest)
        {
            if (updateCartRequest == null) { return BadRequest("updateCartRequest is required"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var getCartParam = new GetCartParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
            };

            var updateCheckoutCartParam = new UpdateCheckoutCartParam
            {
                GetCartParam = getCartParam,
                CurrentStep = updateCartRequest.CurrentStep.GetValueOrDefault(),
                IsGuest = ComposerContext.IsGuest,
                UpdateValues = updateCartRequest.UpdatedCart
            };

            var updateCartResultViewModel = await CheckoutService.UpdateCheckoutCartAsync(updateCheckoutCartParam);

            return Ok(updateCartResultViewModel);
        }

        /// <summary>
        /// Get the shipping methods available for the current cart
        /// </summary>
        /// <returns>A Json representation of the Shipping methods</returns>
        [HttpGet]
        [ActionName("shippingmethods")]
        public virtual async Task<IHttpActionResult> GetShippingMethods()
        {
            var shippingMethodsViewModel = await ShippingMethodService.GetShippingMethodsAsync(new GetShippingMethodsParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName
            });

            return Ok(shippingMethodsViewModel);
        }


        /// <summary>
        /// Get the shipping methods available for the current cart
        /// </summary>
        /// <returns>A Json representation of the Shipping methods</returns>
        [HttpGet]
        [ActionName("groupedshippingmethods")]
        public virtual async Task<IHttpActionResult> GetGroupedShippingMethods()
        {
            var shippingMethodTypesViewModel = await ShippingMethodService.GetShippingMethodTypesAsync(new GetShippingMethodsParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName
            });

            return Ok(shippingMethodTypesViewModel);
        }

        /// <summary>
        /// Get the shipping methods available for a specific cart name
        /// </summary>
        /// <returns>A Json representation of the Shipping methods</returns>
        [HttpPost]
        [ActionName("shippingmethodsbycartname")]
        public async Task<IHttpActionResult> GetShippingMethodsByCartName(GetShippingMethodsByCartNameViewModel request)
        {
            var shippingMethodsViewModel = await ShippingMethodService.GetRecurringCartShippingMethodsAsync(new GetShippingMethodsParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = request.CartName
            });

            return Ok(shippingMethodsViewModel);
        }

        [HttpPut]
        [ActionName("setdefaultpaymentmethod")]
        public virtual async Task<IHttpActionResult> SetCustomerDefaultPaymentMethod(SetCustomerDefaultPaymentMethodViewModel request)
        {
            var paymentMethod = await CartService.SetDefaultCustomerPaymentMethod(new SetDefaultCustomerPaymentMethodParam
            {
                PaymentProviderName = request.PaymentProviderName,
                PaymentMethodId = request.PaymentMethodId,
                CustomerId = ComposerContext.CustomerId,
                ScopeId = ComposerContext.Scope,
                Culture = ComposerContext.CultureInfo
            });

            return Ok(paymentMethod);
        }

        /// <summary>
        /// Get and set the cheapest shipping method for this cart
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("shippingmethod")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> SetShippingMethod(SetShippingMethodViewModel request)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (!request.UseCheapest)
            {
                return new StatusCodeResult(HttpStatusCode.NotImplemented, this);
            }

            var vm = await ShippingMethodService.SetCheapestShippingMethodAsync(new SetCheapestShippingMethodParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope
            });

            return Ok(vm);
        }

        /// <summary>
        /// Add line item to the shopping cart.
        /// 
        /// Cart will be created if needed
        /// If the (product,variant) is already in the cart, the quantity will be increased; otherwise a new line will be added
        /// VariantId is optional
        /// 
        /// param.VariantId is optional
        /// 
        /// </summary>
        /// <param name="request">add args</param>
        /// <returns>A Json representation of the updated cart state</returns>
        [HttpPost]
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> AddLineItem(AddLineItemViewModel request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await CartService.AddLineItemAsync(new AddLineItemParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
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
        /// Remove line item to the cart.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A Json representation of the updated cart state</returns>
        [HttpDelete]
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> RemoveLineItem(RemoveLineItemViewModel request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await CartService.RemoveLineItemAsync(new RemoveLineItemParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                LineItemId = new Guid(request.LineItemId),
                CartName = CartConfiguration.ShoppingCartName,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(vm);
        }

        /// <summary>
        /// Update the line item in the cart.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [ActionName("lineitem")]
        public virtual async Task<IHttpActionResult> UpdateLineItem(UpdateLineItemViewModel request)
        {
            if (request == null) { return BadRequest("Missing Request Body"); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await CartService.UpdateLineItemAsync(new UpdateLineItemParam
            {
                ScopeId = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                LineItemId = new Guid(request.LineItemId),
                CartName = CartConfiguration.ShoppingCartName,
                Quantity = request.Quantity.GetValueOrDefault(),
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                RecurringOrderFrequencyName = request.RecurringOrderFrequencyName,
                RecurringOrderProgramName = request.RecurringOrderProgramName
            });

            return Ok(vm);
        }

        /// <summary>
        /// Update the address of the first shipment of the cart order for taxes and shipping to calculate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("shippingaddress")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> UpdateShippingAddress(UpdateShippingAddressViewModel request)
        {
            if (request == null) { return BadRequest("No body was found on the request."); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await CartService.UpdateShippingAddressPostalCodeAsync(new UpdateShippingAddressPostalCodeParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CartName = CartConfiguration.ShoppingCartName,
                CountryCode = ComposerContext.CountryCode,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                PostalCode = request.PostalCode,
                Scope = ComposerContext.Scope
            });

            return Ok(vm);
        }

        /// <summary>
        /// Update the address of the first valid payment of the cart order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ActionName("billingaddress")]
        [HttpPost]
        public virtual async Task<IHttpActionResult> UpdateBillingAddress(UpdateBillingAddressViewModel request)
        {
            if (request == null) { return BadRequest("No body was found on the request."); }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var vm = await CartService.UpdateBillingAddressPostalCodeAsync(new UpdateBillingAddressPostalCodeParam
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CartName = CartConfiguration.ShoppingCartName,
                CountryCode = ComposerContext.CountryCode,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                PostalCode = request.PostalCode,
                Scope = ComposerContext.Scope
            });

            return Ok(vm);
        }

        [ActionName("coupon")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> AddCouponAsync([FromBody]CouponRequest request)
        {
            if (request == null) { return BadRequest("No request found."); }

            var vm = await CouponViewService.AddCouponAsync(new CouponParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                Scope = ComposerContext.Scope,
                CouponCode = request.CouponCode,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(vm);
        }

        [ActionName("coupon")]
        [HttpDelete]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> RemoveCouponAsync(CouponRequest request)
        {
            if (request == null) { return BadRequest("No request found."); }

            var vm = await CouponViewService.RemoveCouponAsync(new CouponParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                Scope = ComposerContext.Scope,
                CouponCode = request.CouponCode,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            return Ok(vm);
        }

        [ActionName("clean")]
        [HttpDelete]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> CleanCartAsync()
        {
            var param = new RemoveInvalidLineItemsParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            };

            var cartViewModel = await CartService.RemoveInvalidLineItemsAsync(param);

            return Ok(cartViewModel);
        }

        protected virtual string GetCheckoutSignInUrl()
        {
            var getCartUrlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };

            return CartUrlProvider.GetCheckoutSignInUrl(getCartUrlParam);
        }

        protected virtual string GetCartUrl()
        {
            var getCartUrlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };

            return CartUrlProvider.GetCartUrl(getCartUrlParam);
        }

        [ActionName("completecheckout")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> CompleteCheckout()
        {
 
            var checkoutViewModel = await CheckoutService.CompleteCheckoutAsync(new CompleteCheckoutParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
            });

            checkoutViewModel.NextStepUrl = CartUrlProvider.GetCheckoutConfirmationPageUrl(
                new BaseUrlParameter { CultureInfo = ComposerContext.CultureInfo });

            checkoutViewModel.IsAuthenticated = ComposerContext.IsAuthenticated;

            return Ok(checkoutViewModel);
        }

        /// <summary>
        /// Get the shipping methods available for the scope 
        /// </summary>
        /// <returns>A Json representation of the Shipping methods</returns>
        [HttpGet]
        [ActionName("shippingmethodsscope")]
        public async Task<IHttpActionResult> GetShippingMethodsScope()
        {
            var shippingMethodsViewModel = await ShippingMethodService.GetShippingMethodsScopeAsync(new GetShippingMethodsScopeParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo
            }).ConfigureAwait(false);

            return Ok(shippingMethodsViewModel);
        }
    }
}