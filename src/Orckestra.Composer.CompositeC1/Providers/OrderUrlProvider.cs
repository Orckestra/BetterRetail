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
        protected IWebsiteContext WebsiteContext { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }

        public OrderUrlProvider(IPageService pageService, IWebsiteContext wbsiteContext, ISiteConfiguration siteConfiguration)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            WebsiteContext = wbsiteContext;
            SiteConfiguration = siteConfiguration;
        }

        public string GetOrderDetailsBaseUrl(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }
 
            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.OrderDetailsPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetOrderHistoryUrl(GetOrderUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.OrderHistoryPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetGuestOrderDetailsUrl(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.GuestOrderDetailsPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetFindMyOrderUrl(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.FindMyOrderPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }
    }
}