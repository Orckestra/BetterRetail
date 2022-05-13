using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Website.Controllers
{
    public class CartController : CartBaseController
    {
        public CartController(
            IComposerContext composerContext, 
            ICartUrlProvider cartUrlProvider, 
            IPageService pageService) 

            : base(
            composerContext, 
            cartUrlProvider, 
            pageService) { }
    }
}