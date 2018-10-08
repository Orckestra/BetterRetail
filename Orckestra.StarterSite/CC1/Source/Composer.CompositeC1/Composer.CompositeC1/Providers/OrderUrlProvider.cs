using System;
using System.Globalization;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class OrderUrlProvider : IOrderUrlProvider
    {
        protected IPageService PageService { get; private set; }

        public OrderUrlProvider(IPageService pageService)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }

            PageService = pageService;
        }

        public string GetOrderDetailsBaseUrl(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }

            var url = PageService.GetPageUrl(PagesConfiguration.OrderDetailsPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetOrderHistoryUrl(GetOrderUrlParameter param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(PagesConfiguration.OrderHistoryPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetGuestOrderDetailsUrl(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }

            var url = PageService.GetPageUrl(PagesConfiguration.GuestOrderDetailsPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }

        public string GetFindMyOrderUrl(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) { throw new ArgumentNullException("cultureInfo"); }

            var url = PageService.GetPageUrl(PagesConfiguration.FindMyOrderPageId, cultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, null);
        }
    }
}
