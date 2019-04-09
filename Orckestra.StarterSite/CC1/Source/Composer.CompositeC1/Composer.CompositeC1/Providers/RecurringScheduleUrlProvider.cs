using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using System;

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
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(PagesConfiguration.RecurringSchedulePageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }
    }
}
