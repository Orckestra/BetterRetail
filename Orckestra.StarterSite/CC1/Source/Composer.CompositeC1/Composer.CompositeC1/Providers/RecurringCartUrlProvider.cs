using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class RecurringCartUrlProvider : IRecurringCartUrlProvider
    {
        protected IPageService PageService { get; private set; }

        public RecurringCartUrlProvider(IPageService pageService)
        {
            if (pageService == null) { throw new ArgumentNullException("pageService"); }

            PageService = pageService;
        }
        public string GetRecurringCartsUrl(GetRecurringCartsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var url = PageService.GetPageUrl(PagesConfiguration.RecurringCartsPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringCartDetailsUrl(GetRecurringCartDetailsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.RecurringCartName == null) { throw new ArgumentNullException(nameof(param.RecurringCartName)); }

            var url = PageService.GetPageUrl(PagesConfiguration.RecurringCartDetailsPageId, param.CultureInfo);
            var UrlWithReturn = UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);

            return UrlFormatter.AppendQueryString(UrlWithReturn, new NameValueCollection
                {
                    {"name", param.RecurringCartName }
                });
        }

        public string GetRecurringCartAddAddressUrl(GetRecurringCartsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(PagesConfiguration.AddAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

        public string GetRecurringCarUpdateAddressBaseUrl(GetRecurringCartsUrlParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            var url = PageService.GetPageUrl(PagesConfiguration.UpdateAddressPageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }

    }
}
