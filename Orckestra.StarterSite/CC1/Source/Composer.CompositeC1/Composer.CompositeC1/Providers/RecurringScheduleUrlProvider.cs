using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using System;
using System.Collections.Specialized;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class RecurringScheduleUrlProvider : IRecurringScheduleUrlProvider
    {
        protected IPageService PageService { get; private set; }

        public RecurringScheduleUrlProvider(IPageService pageService)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }

            PageService = pageService;
        }
        /// <summary>
        /// Get the Url of the Recurring Schedule page.
        /// </summary>
        public string GetRecurringScheduleUrl(GetRecurringScheduleUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var url = PageService.GetPageUrl(PagesConfiguration.RecurringSchedulePageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringScheduleDetailsBaseUrl(GetRecurringScheduleDetailsBaseUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var url = PageService.GetPageUrl(PagesConfiguration.RecurringScheduleDetailsPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringScheduleDetailsUrl(GetRecurringScheduleDetailsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.RecurringScheduleId == null) { throw new ArgumentNullException(nameof(param.RecurringScheduleId)); }

            var url = PageService.GetPageUrl(PagesConfiguration.RecurringScheduleDetailsPageId, param.CultureInfo);
            var UrlWithReturn = UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);

            return UrlFormatter.AppendQueryString(UrlWithReturn, new NameValueCollection
                {
                    {"id", param.RecurringScheduleId }
                });
        }
    }
}
