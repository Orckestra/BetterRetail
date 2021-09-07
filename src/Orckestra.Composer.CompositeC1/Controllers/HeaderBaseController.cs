using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using System;
using System.Globalization;
using System.Net.Http;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class HeaderBaseController : Controller
    {
        protected IPageService PageService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }

        protected HeaderBaseController(
            IPageService pageService,
            IComposerContext composerContext,
            ILanguageSwitchService languageSwitchService
            )
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            LanguageSwitchService = languageSwitchService ?? throw new ArgumentNullException(nameof(languageSwitchService));
        }

        public virtual ActionResult LanguageSwitch()
        {
            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(BuildUrl, ComposerContext.CultureInfo);

            return View("LanguageSwitch", languageSwitchViewModel);
        }

        private string BuildUrl(CultureInfo culture)
        {
            var pageId = SitemapNavigator.CurrentPageId;
            var pageUrl = PageService.GetPageUrl(pageId, culture);

            if (pageUrl == null) { return null; }

            var url = UrlFormatter.AppendQueryString(pageUrl, Request.Url.ParseQueryString());
            return url;
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
