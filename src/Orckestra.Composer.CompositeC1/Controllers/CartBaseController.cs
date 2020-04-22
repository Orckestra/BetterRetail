using Composite.Data;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
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
        protected IBreadcrumbViewService BreadcrumbViewService { get; private set; }

        protected CartBaseController(
            IComposerContext composerContext,
            ICartUrlProvider cartUrlProvider,
            IPageService pageService,
            IBreadcrumbViewService breadcrumbViewService)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (cartUrlProvider == null) { throw new ArgumentNullException("cartUrlProvider"); }
            if (pageService == null) { throw new ArgumentNullException("pageService"); }
            if (breadcrumbViewService == null) { throw new ArgumentNullException("breadcrumbViewService"); }

            ComposerContext = composerContext;
            CartUrlProvider = cartUrlProvider;
            PageService = pageService;
            BreadcrumbViewService = breadcrumbViewService;
        }

        public virtual ActionResult CartSummary()
        {
            var cartViewModel = GetCartViewModel();

            if (cartViewModel == null) { return View("CartSummaryBlade", (CartViewModel) null); }

            return View("CartSummaryBlade", cartViewModel);
        }

        public virtual ActionResult OrderSummary()
        {
            var cartViewModel = GetCartViewModel();

            return View("OrderSummaryBlade", cartViewModel);
        }

        public virtual ActionResult Coupons()
        {
            var cartViewModel = GetCartViewModel();

            return View("CouponsBlade", cartViewModel);
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

        public virtual ActionResult Breadcrumb()
        {
            var breadcrumbViewModel = BreadcrumbViewService.CreateBreadcrumbViewModel(new GetBreadcrumbParam
            {
                CurrentPageId = SitemapNavigator.CurrentPageId.ToString(),
                CultureInfo = ComposerContext.CultureInfo
            });

            return View(breadcrumbViewModel);
        }
    }
}
