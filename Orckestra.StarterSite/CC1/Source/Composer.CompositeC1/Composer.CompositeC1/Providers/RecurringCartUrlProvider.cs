using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Collections.Specialized;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class RecurringCartUrlProvider : IRecurringCartUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }

        public RecurringCartUrlProvider(IPageService pageService, IWebsiteContext websiteContext)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }

            PageService = pageService;
            WebsiteContext = websiteContext;

        }
        public string GetRecurringCartsUrl(GetRecurringCartsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, WebsiteContext.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.RecurringCartsPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringCartDetailsUrl(GetRecurringCartDetailsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.RecurringCartName == null) { throw new ArgumentNullException(nameof(param.RecurringCartName)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, WebsiteContext.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.RecurringCartDetailsPageId, param.CultureInfo);
            var UrlWithReturn = UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);

            return UrlFormatter.AppendQueryString(UrlWithReturn, new NameValueCollection
                {
                    {"name", param.RecurringCartName }
                });
        }
    }
}
