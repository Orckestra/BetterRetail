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
    public class RecurringScheduleUrlProvider : IRecurringScheduleUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected IWebsiteContext WebsiteContext { get; private set; }

        public RecurringScheduleUrlProvider(IPageService pageService, IWebsiteContext websiteContext)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }

            PageService = pageService;
            WebsiteContext = websiteContext;
        }
        /// <summary>
        /// Get the Url of the Recurring Schedule page.
        /// </summary>
        public string GetRecurringScheduleUrl(GetRecurringScheduleUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, WebsiteContext.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.RecurringSchedulePageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringScheduleDetailsBaseUrl(GetRecurringScheduleDetailsBaseUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, WebsiteContext.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.RecurringScheduleDetailsPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringScheduleDetailsUrl(GetRecurringScheduleDetailsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.RecurringScheduleId == null) { throw new ArgumentNullException(nameof(param.RecurringScheduleId)); }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(param.CultureInfo, WebsiteContext.WebsiteId);
            var url = PageService.GetPageUrl(pagesConfiguration.RecurringScheduleDetailsPageId, param.CultureInfo);
            var UrlWithReturn = UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);

            return UrlFormatter.AppendQueryString(UrlWithReturn, new NameValueCollection
                {
                    {"id", param.RecurringScheduleId }
                });
        }
    }
}
