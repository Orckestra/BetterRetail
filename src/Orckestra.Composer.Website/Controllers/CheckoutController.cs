using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;

namespace Orckestra.Composer.Website.Controllers
{
    public class CheckoutController : CheckoutBaseController
    {
        public CheckoutController(
            IPageService pageService, 
            IComposerContext composerContext, 
            ICheckoutBreadcrumbViewService confirmationBreadcrumbViewService, 
            IBreadcrumbViewService breadcrumbViewService, 
            ILanguageSwitchService languageSwitchService, 
            ICartUrlProvider urlProvider, 
            ICheckoutNavigationViewService checkoutNavigationViewService, 
            IPaymentViewService paymentViewService, 
            IMyAccountUrlProvider myAccountUrlProvider, 
            ICartService cartService,
			IWebsiteContext websiteContext) : 
            base(pageService, 
                composerContext, 
                confirmationBreadcrumbViewService, 
                breadcrumbViewService, 
                languageSwitchService, 
                urlProvider, 
                checkoutNavigationViewService, 
                paymentViewService,
                myAccountUrlProvider,
                cartService,
				websiteContext)
        {
        }
    }
}