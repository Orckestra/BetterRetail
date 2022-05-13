using System;
using System.Collections.Specialized;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class RecurringScheduleUrlProvider : IRecurringScheduleUrlProvider
    {
        protected IPageService PageService { get; private set; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        public RecurringScheduleUrlProvider(IPageService pageService, IRecurringOrdersSettings recurringOrdersSettings)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            RecurringOrdersSettings = recurringOrdersSettings;
        }
        /// <summary>
        /// Get the Url of the Recurring Schedule page.
        /// </summary>
        public string GetRecurringScheduleUrl(GetRecurringScheduleUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var url = PageService.GetPageUrl(RecurringOrdersSettings.RecurringSchedulePageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringScheduleDetailsBaseUrl(GetRecurringScheduleDetailsBaseUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var url = PageService.GetPageUrl(RecurringOrdersSettings.RecurringScheduleDetailsPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringScheduleDetailsUrl(GetRecurringScheduleDetailsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.RecurringScheduleId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.RecurringScheduleId)), nameof(param)); }

            var url = PageService.GetPageUrl(RecurringOrdersSettings.RecurringScheduleDetailsPageId, param.CultureInfo);
            var UrlWithReturn = UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);

            return UrlFormatter.AppendQueryString(UrlWithReturn, new NameValueCollection
                {
                    {"id", param.RecurringScheduleId }
                });
        }
    }
}
