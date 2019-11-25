using Orckestra.Composer.CompositeC1.Controllers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;

namespace Orckestra.Composer.CompositeC1.Mvc.Controllers
{
    public class CartController : CartBaseController
    {
        public CartController(
            IComposerContext composerContext, 
            ICartUrlProvider cartUrlProvider, 
            IPageService pageService,
            IBreadcrumbViewService breadcrumbViewService) 

            : base(
            composerContext, 
            cartUrlProvider, 
            pageService,
            breadcrumbViewService)
        {
        }
    }
}