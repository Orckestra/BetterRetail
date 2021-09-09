using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using System;
using System.Web.Mvc;
using ActionResult = System.Web.Mvc.ActionResult;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class CartBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ICartUrlProvider CartUrlProvider { get; private set; }
        protected IPageService PageService { get; private set; }

        protected CartBaseController(
            IComposerContext composerContext,
            ICartUrlProvider cartUrlProvider,
            IPageService pageService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CartUrlProvider = cartUrlProvider ?? throw new ArgumentNullException(nameof(cartUrlProvider));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
        }

        protected virtual CartViewModel GetCartViewModel()
        {
            var cartViewModel = new CartViewModel
            {
                IsLoading = true,
                HomepageUrl = CartUrlProvider.GetHomepageUrl(new BaseUrlParameter
                {                    
                    CultureInfo = ComposerContext.CultureInfo
                })
            };

            return cartViewModel;
        }

        public virtual ActionResult Minicart(int notificationTimeInSeconds)
        {
            var minicartViewModel = new MinicartViewModel
            {
                NotificationTimeInMilliseconds = notificationTimeInSeconds * 1000,
                Url = CartUrlProvider.GetCartUrl(new BaseUrlParameter
                {                    
                    CultureInfo = ComposerContext.CultureInfo,
                 })
            };

            return View(minicartViewModel);
        }
    }
}