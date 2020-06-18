using System;
using System.Globalization;
using System.Net.Http;
using System.Web.Mvc;
using Composite.Core;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.CompositeC1.Utils;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class HeaderBaseController : Controller
    {
        protected IPageService PageService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IBreadcrumbViewService BreadcrumbViewService { get; private set; }
        protected ILanguageFallbackProvider LanguageFallbackProvider { get; private set; }

        protected HeaderBaseController(
            IPageService pageService,
            IComposerContext composerContext,
            ILanguageSwitchService languageSwitchService,
            IBreadcrumbViewService breadcrumbViewService
        )
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            LanguageSwitchService = languageSwitchService ?? throw new ArgumentNullException(nameof(languageSwitchService));
            BreadcrumbViewService = breadcrumbViewService ?? throw new ArgumentNullException(nameof(breadcrumbViewService));
            LanguageFallbackProvider = ServiceLocator.GetService<ILanguageFallbackProvider>() ?? throw new ArgumentNullException(nameof(LanguageFallbackProvider));
        }

        public virtual ActionResult LanguageSwitch()
        {
            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(BuildUrl, ComposerContext.CultureInfo);

            return View("LanguageSwitch", languageSwitchViewModel);
        }

        private string BuildUrl(CultureInfo culture)
        {
            
            using (LanguageFallbackProvider.GetInvariantLanguageScope())
            {
                var pageId = SitemapNavigator.CurrentPageId;

                using (var connection = new DataConnection(culture))
                {
                    var page = connection.SitemapNavigator.GetPageNodeById(pageId);
                    if (page == null)
                    {
                        return null;
                    }
                    var pageUrl = page.Url;

                    if (pageUrl == null)
                    {
                        return null;
                    }

                    var url = UrlFormatter.AppendQueryString(pageUrl, Request.Url.ParseQueryString());
                    return url;
                }
            }
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
            var page = PageService.GetPageNode(SitemapNavigator.CurrentPageId);
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
