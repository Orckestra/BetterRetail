using Orckestra.Composer.Cart;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.MvcFilters;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using System;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    [ValidateReturnUrl]
    public abstract class CheckoutBaseController : Controller
    {
        protected IPageService PageService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ICartUrlProvider UrlProvider { get; private set; }
        protected IMyAccountUrlProvider MyAccountUrlProvider { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected ICartService CartService { get; private set; }

        protected CheckoutBaseController(
            IPageService pageService,
            IComposerContext composerContext,
            ICartUrlProvider urlProvider,
            IMyAccountUrlProvider myAccountUrlProvider,
            ICartService cartService,
            IWebsiteContext websiteContext
            )
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            UrlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
            MyAccountUrlProvider = myAccountUrlProvider ?? throw new ArgumentNullException(nameof(myAccountUrlProvider));
            WebsiteContext = websiteContext;
            CartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        [MustBeAnonymous(MustBeAnonymousAttribute.CartDestination)]
        public virtual ActionResult CheckoutSignInAsGuest()
        {
            var checkoutUrl = UrlProvider.GetCheckoutPageUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            });

            var registerUrl = MyAccountUrlProvider.GetCreateAccountUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo,
                ReturnUrl = checkoutUrl
            });

            var cart = CartService.GetCartViewModelAsync(new GetCartParam()
            {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CartName = CartConfiguration.ShoppingCartName,
                CultureInfo = ComposerContext.CultureInfo,
                CustomerId = ComposerContext.CustomerId,
                ExecuteWorkflow = true,
                Scope = ComposerContext.Scope
            }).Result;

            var hasRecurringItems = cart.HasRecurringLineitems;

            var checkoutSignInAsGuestViewModel = new CheckoutSignInAsGuestViewModel
            {
                CheckoutUrlTarget = checkoutUrl,
                RegisterUrl = registerUrl,
                IsCartContainsRecurringLineitems = hasRecurringItems
            };

            return View("CheckoutSignInAsGuest", checkoutSignInAsGuestViewModel);
        }

        [MustBeAnonymous(MustBeAnonymousAttribute.CartDestination)]
        public virtual ActionResult CheckoutSignInAsCustomer()
        {
            var forgotPasswordUrl = MyAccountUrlProvider.GetForgotPasswordUrl(new BaseUrlParameter
            {
                CultureInfo = ComposerContext.CultureInfo
            });

            var vm = new CheckoutSignInAsReturningViewModel
            {
                ForgotPasswordUrl = forgotPasswordUrl
            };

            return View("ReturningCustomerBlade", vm);
        }

        protected virtual CartViewModel BuildCartViewModel()
        {
            var cartViewModel = new CartViewModel
            {
                GettingCart = true,
                IsAuthenticated = ComposerContext.IsAuthenticated
            };

            return cartViewModel;
        }
    }
}