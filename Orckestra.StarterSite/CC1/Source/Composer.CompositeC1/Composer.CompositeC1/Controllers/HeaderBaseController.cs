using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.CompositeC1.Utils;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class HeaderBaseController : Controller
    {
        protected IPageService PageService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IHomeViewService HomeViewService { get; private set; }
        protected IBreadcrumbViewService BreadcrumbViewService { get; private set; }

        protected HeaderBaseController(
            IPageService pageService,
            IComposerContext composerContext, 
            ILanguageSwitchService languageSwitchService,
            IHomeViewService homeViewService,
            IBreadcrumbViewService breadcrumbViewService
            )
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (languageSwitchService == null) { throw new ArgumentNullException("languageSwitchService"); }
            if (homeViewService == null) { throw new ArgumentNullException("homeViewService"); }
            if (breadcrumbViewService == null) { throw new ArgumentNullException("breadcrumbViewService"); }

            PageService = pageService;
            ComposerContext = composerContext;
            LanguageSwitchService = languageSwitchService;
            HomeViewService = homeViewService;
            BreadcrumbViewService = breadcrumbViewService;
        }

        public virtual ActionResult MainMenu()
        {
            var param = new GetHomeMainMenuParam
            {
                CultureInfo = ComposerContext.CultureInfo,
            };

            var mainMenuViewModel = HomeViewService.GetHomeMainMenuViewModel(param).Result;

            return View("MainMenu", mainMenuViewModel);
        }

        public virtual ActionResult StickyLinks()
        {
            var param = new GetStickyLinksParam
            {
                CultureInfo = ComposerContext.CultureInfo,
            };

            var viewModel = HomeViewService.GetStickyLinksViewModel(param).Result;

            return View("StickyLinks", viewModel);
        }

        public virtual ActionResult LanguageSwitch()
        {
            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(BuildUrl, ComposerContext.CultureInfo);
            
            return View("LanguageSwitch", languageSwitchViewModel);
        }

        public virtual ActionResult OptionalLinks()
        {
            var optionalLinksViewModel = HomeViewService.GetOptionalLinksViewModel(ComposerContext.CultureInfo).Result;

            return View("OptionalLinks", optionalLinksViewModel);
        }

        private string BuildUrl(CultureInfo culture)
        {
            var pageId = SitemapNavigator.CurrentPageId;
            var pageUrl = PageService.GetPageUrl(pageId, culture);

            if(pageUrl == null) { return null; }

            var url = UrlFormatter.AppendQueryString(pageUrl, Request.Url.ParseQueryString());
            return url;
        }


        public virtual ActionResult Breadcrumb()
        {
            var breadcrumbVm = BreadcrumbViewService.CreateBreadcrumbViewModel(new GetBreadcrumbParam
            {
                CurrentPageId = SitemapNavigator.CurrentPageId.ToString(),
                CultureInfo = ComposerContext.CultureInfo
            });

            return View(breadcrumbVm);
        }

        public virtual ActionResult PageHeader()
        {
            var page = PageService.GetPage(SitemapNavigator.CurrentPageId);
            var canonicalUrl = page != null ? UrlUtils.ToAbsolute(PageService.GetPageUrl(page.Id)) : string.Empty;

            var pageHeaderViewModel = new PageHeaderViewModel
            {
                PageTitle = page.Title,
                NoIndex = string.IsNullOrWhiteSpace(canonicalUrl),
                CanonicalUrl = canonicalUrl
            };

            return View("PageHeader", pageHeaderViewModel);
        }

        protected virtual bool IsPageIndexed()
        {
            return !Request.QueryString.HasKeys();
        }

        public virtual ActionResult GeneralErrors()
        {
            return View("GeneralErrorContainer");
        }
    }
}
