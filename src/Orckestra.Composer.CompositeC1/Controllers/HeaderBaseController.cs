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

        protected HeaderBaseController(
            IPageService pageService,
            IComposerContext composerContext)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        public virtual ActionResult PageHeader()
        {
            using (var connection = new DataConnection())
            {
                var currentPageNode = connection.SitemapNavigator.CurrentPageNode;
                var pageUrl = currentPageNode?.Url;
                var canonicalUrl = !string.IsNullOrEmpty(pageUrl) ? UrlUtils.ToAbsolute(pageUrl) : string.Empty;
                var pageHeaderViewModel = new PageHeaderViewModel
                {
                    PageTitle = currentPageNode?.Title,
                    NoIndex = string.IsNullOrWhiteSpace(canonicalUrl),
                    CanonicalUrl = canonicalUrl
                };

                return View("PageHeader", pageHeaderViewModel);
            }
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
