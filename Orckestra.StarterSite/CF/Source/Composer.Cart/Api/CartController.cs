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
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Cart.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class CartController : ApiController
    {
        protected ICartService CartService { get; private set; }
        protected IComposerRequestContext ComposerContext { get; private set; }
        protected ICouponViewService CouponViewService { get; private set; }
        protected ICheckoutService CheckoutService { get; private set; }
        protected IShippingMethodViewService ShippingMethodService { get; private set; }
        protected ICartUrlProvider CartUrlProvider { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        public CartController(
            ICartService cartService,
            ICheckoutService checkoutService,
            IComposerRequestContext composerContext,
            ICouponViewService couponViewService,
            IShippingMethodViewService shippingMethodService,
            ICartUrlProvider cartUrlProvider,
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            if (cartService == null) { throw new ArgumentNullException("cartService"); }
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (couponViewService == null) { throw new ArgumentNullException("couponViewService"); }
            if (checkoutService == null) { throw new ArgumentNullException("checkoutService"); }
            if (shippingMethodService == null) { throw new ArgumentNullException("shippingMethodService"); }
            if (cartUrlProvider == null) { throw new ArgumentNullException("cartUrlProvider"); }

            CartService = cartService;
            ComposerContext = composerContext;
            CouponViewService = couponViewService;
            CheckoutService = checkoutService;
            ShippingMethodService = shippingMethodService;
            CartUrlProvider = cartUrlProvider;
            RecurringOrdersSettings = recurringOrdersSettings;
        }

        /// <summary>
        /// Get the shopping cart for the current customer
        /// </summary>
        /// <returns>A Json representation of cart state</returns>
        [HttpGet]
        [ActionName("getcart")]
        public virtual async Task<IHttpActionResult> GetCart()
        {

            var homepageUrl = GetHomepageUrl();            

            var cartViewModel = await CartService.GetCartViewModelAsync(new GetCartParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
            });

            SetHomepageUrl(cartViewModel, homepageUrl);
            SetEditCartUrl(cartViewModel);

            if (cartViewModel.OrderSummary != null)
            {
                var checkoutUrlTarget = GetCheckoutUrl();
                var checkoutStepInfos = CartUrlProvider.GetCheckoutStepPageInfos(new BaseUrlParameter
                {
                    CultureInfo = ComposerContext.CultureInfo
                });
                //Build redirect url in case of the customer try to access an unauthorized step.
                var stepNumber = cartViewModel.OrderSummary.CheckoutRedirectAction.LastCheckoutStep;
                if (!checkoutStepInfos.ContainsKey(stepNumber))
                {
                    return BadRequest("StepNumber is invalid");
                }

                var checkoutStepInfo = checkoutStepInfos[stepNumber];

                //If the cart contains recurring items and user is not authenticated, redirect to sign in
                if (RecurringOrdersSettings.Enabled && cartViewModel.HasRecurringLineitems && !ComposerContext.IsAuthenticated)
                {
                    cartViewModel.OrderSummary.CheckoutRedirectAction.RedirectUrl = checkoutUrlTarget;
                }
                else
                {
                    cartViewModel.OrderSummary.CheckoutRedirectAction.RedirectUrl = checkoutStepInfo.Url;
                }
                cartViewModel.OrderSummary.CheckoutStepUrls = checkoutStepInfos.Values.Select(x => x.Url).ToList();
                cartViewModel.OrderSummary.CheckoutUrlTarget = checkoutUrlTarget;
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

            var getCartUrlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };

            var checkoutStepInfos = CartUrlProvider.GetCheckoutStepPageInfos(getCartUrlParam);

            var nextStepUrl = CartUrlProvider.GetCheckoutStepUrl(new GetCheckoutStepUrlParam
            {                
                CultureInfo = ComposerContext.CultureInfo,
                StepNumber = updateCartRequest.CurrentStep.GetValueOrDefault() + 1
            });

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

            var homepageUrl = GetHomepageUrl();

            var updateCartResultViewModel = await CheckoutService.UpdateCheckoutCartAsync(updateCheckoutCartParam);

            SetHomepageUrl(updateCartResultViewModel.Cart, homepageUrl);
            SetEditCartUrl(updateCartResultViewModel.Cart);

            if (updateCartResultViewModel.Cart.OrderSummary != null)
            {                
                updateCartResultViewModel.Cart.OrderSummary.CheckoutStepUrls = checkoutStepInfos.Values.Select(x => x.Url).ToList();
            }

            if (!updateCartResultViewModel.HasErrors)
            {
                updateCartResultViewModel.NextStepUrl = nextStepUrl;
            }

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

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

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

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

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

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

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

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

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

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

            var vm = await CartService.RemoveLineItemAsync(new RemoveLineItemParam
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                LineItemId = new Guid(request.LineItemId),
                CartName = CartConfiguration.ShoppingCartName,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

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

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

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

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

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

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

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

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

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

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

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

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

            return Ok(vm);
        }

        [ActionName("coupon")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> AddCouponAsync([FromBody]CouponRequest request)
        {
            if (request == null) { return BadRequest("No request found."); }

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

            var vm = await CouponViewService.AddCouponAsync(new CouponParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                Scope = ComposerContext.Scope,
                CouponCode = request.CouponCode,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

            return Ok(vm);
        }

        [ActionName("coupon")]
        [HttpDelete]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> RemoveCouponAsync(CouponRequest request)
        {
            if (request == null) { return BadRequest("No request found."); }

            var checkoutUrl = GetCheckoutUrl();
            var homepageUrl = GetHomepageUrl();

            var vm = await CouponViewService.RemoveCouponAsync(new CouponParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                Scope = ComposerContext.Scope,
                CouponCode = request.CouponCode,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString()
            });

            SetCheckoutUrl(vm, checkoutUrl);
            SetHomepageUrl(vm, homepageUrl);
            SetEditCartUrl(vm);

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

        protected virtual string GetCheckoutUrl()
        {
            var getCartUrlParam = new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            };

            var checkoutStepInfos = CartUrlProvider.GetCheckoutStepPageInfos(getCartUrlParam);

            var checkoutSignInUrl = CartUrlProvider.GetCheckoutSignInUrl(new BaseUrlParameter
            {                
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = ComposerContext.IsAuthenticated ? null : checkoutStepInfos[1].Url
            });

            var checkoutUrlTarget = ComposerContext.IsAuthenticated ? checkoutStepInfos[1].Url : checkoutSignInUrl;

            return checkoutUrlTarget;
        }

        protected virtual void SetCheckoutUrl(CartViewModel cartViewModel, string checkoutUrl)
        {
            if (cartViewModel.OrderSummary != null)
            {
                cartViewModel.OrderSummary.CheckoutUrlTarget = checkoutUrl;
            }
        }

        protected virtual string GetHomepageUrl()
        {
            var homepageUrl = CartUrlProvider.GetHomepageUrl(new BaseUrlParameter
            {                
                CultureInfo = ComposerContext.CultureInfo
            });

            return homepageUrl;
        }

        protected virtual void SetHomepageUrl(CartViewModel cartViewModel, string homepageUrl)
        {
            if (cartViewModel != null)
            {
                cartViewModel.HomepageUrl = homepageUrl;
            }
        }

        protected void SetEditCartUrl(CartViewModel cartViewModel)
        {
            if (cartViewModel.OrderSummary != null)
            {
                var getCartUrlParam = new BaseUrlParameter
                {
                    CultureInfo = ComposerContext.CultureInfo
                };

                var cartUrl = CartUrlProvider.GetCartUrl(getCartUrlParam);

                cartViewModel.OrderSummary.EditCartUrlTarget = cartUrl;
            }
        }

        [ActionName("completecheckout")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> CompleteCheckout(CompleteCheckoutRequest request)
        {
            if (request == null) { return BadRequest("No request body found."); }

            var nextStepUrl = CartUrlProvider.GetCheckoutStepUrl(new GetCheckoutStepUrlParam
            {                
                CultureInfo = ComposerContext.CultureInfo,
                StepNumber = request.CurrentStep + 1
            });

            var checkoutViewModel = await CheckoutService.CompleteCheckoutAsync(new CompleteCheckoutParam
            {
                CartName = CartConfiguration.ShoppingCartName,
                CustomerId = ComposerContext.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
            });

            checkoutViewModel.NextStepUrl = nextStepUrl;

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
