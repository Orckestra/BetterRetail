using System;
using System.Globalization;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class OrderUrlProvider : IOrderUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }

        public OrderUrlProvider(IPageService pageService, IComposerContext composerContext)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }

            PageService = pageService;
            ComposerContext = composerContext;
        }

        public string GetOrderDetailsBaseUrl(CultureInfo cultureInfo, Guid websiteId)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }
            if (websiteId == null) { throw new ArgumentNullException(nameof(websiteId)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, websiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.OrderDetailsPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetOrderHistoryUrl(GetOrderUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, param.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.OrderHistoryPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetGuestOrderDetailsUrl(CultureInfo cultureInfo, Guid websiteId)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }
            if (websiteId == null) { throw new ArgumentNullException(nameof(websiteId)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, websiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.GuestOrderDetailsPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetFindMyOrderUrl(CultureInfo cultureInfo, Guid websiteId)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }
            if (websiteId == null) { throw new ArgumentNullException(nameof(websiteId)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, websiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.FindMyOrderPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }
    }
}
