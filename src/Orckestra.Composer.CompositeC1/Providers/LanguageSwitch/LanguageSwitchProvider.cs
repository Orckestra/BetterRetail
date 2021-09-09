using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels.LanguageSwitch;
using System;
using System.Globalization;
using System.Net.Http;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Providers.LanguageSwitch
{
    public class LanguageSwitchProvider : ILanguageSwitchProvider
    {
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IPageService PageService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        protected HttpRequestBase Request { get; }

        public LanguageSwitchProvider(ILanguageSwitchService viewService, IComposerContext composerContext, IPageService pageService, HttpRequestBase request)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            LanguageSwitchService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            return true;
        }

        public LanguageSwitchViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            Func<CultureInfo, string> BuildUrl = (CultureInfo culture) =>
            {
                var pageUrl = PageService.GetPageUrl(currentPageId, culture);
                if (pageUrl == null) { return null; }
                var url = UrlFormatter.AppendQueryString(pageUrl, Request.Url.ParseQueryString());
                return url;
            };

            return LanguageSwitchService.GetViewModel(BuildUrl, ComposerContext.CultureInfo);
        }

    }
}
