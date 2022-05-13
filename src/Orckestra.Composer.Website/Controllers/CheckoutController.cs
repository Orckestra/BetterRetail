using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class CheckoutController : CheckoutBaseController
    {
        public CheckoutController(
            IPageService pageService,
            IComposerContext composerContext,
            ICartUrlProvider urlProvider,
            IMyAccountUrlProvider myAccountUrlProvider,
            ICartService cartService,
            IWebsiteContext websiteContext) :
            base(pageService,
                composerContext,
                urlProvider,
                myAccountUrlProvider,
                cartService,
                websiteContext)
        {
        }
    }
}