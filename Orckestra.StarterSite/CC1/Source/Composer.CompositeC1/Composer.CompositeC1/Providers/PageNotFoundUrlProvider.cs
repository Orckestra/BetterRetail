using System;
using System.Globalization;
using System.Linq;
using System.Web;
using Composite.Core;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.CompositeC1.Utils;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class PageNotFoundUrlProvider : IPageNotFoundUrlProvider
    {
        const string ErrorPathQuerystringName = "errorpath";

        protected IPageService PageService { get; private set; }

        public PageNotFoundUrlProvider(IPageService pageService)
        {
            if (pageService == null)
            {
                throw new ArgumentNullException(nameof(pageService));
            }

            PageService = pageService;
        }

        public virtual string Get404PageUrl(string requestedPath)
        {
            if (requestedPath == null)
            {
                throw new ArgumentException("Requested Path is required", nameof(requestedPath));
            }

            var culture = GetCultureInfo(requestedPath);
            if (culture == null)
            {
                return null;
            }
            var url = PageService.GetPageUrl(PagesConfiguration.PageNotFoundPageId, culture);
            var urlBuilder = new UrlBuilder(url) {[ErrorPathQuerystringName] = HttpUtility.UrlEncode(requestedPath) };

            return urlBuilder.ToString();
        }

        //TODO: Asked to make C1's utils method, so remove that when it is done.
        private CultureInfo GetCultureInfo(string requestedUrl)
        {
            var urlMappingName = UrlUtils.ParseUrlMappingName(requestedUrl);

            if (!string.IsNullOrWhiteSpace(urlMappingName) &&
                DataLocalizationFacade.UrlMappingNames.Any(
                    um => string.Equals(um, urlMappingName, StringComparison.OrdinalIgnoreCase)))
            {
                var cultureInfo = DataLocalizationFacade.GetCultureInfoByUrlMappingName(urlMappingName);

                if (DataLocalizationFacade.ActiveLocalizationNames.Contains(cultureInfo.Name))
                {
                    return cultureInfo;
                }
            }

            return DataLocalizationFacade.DefaultLocalizationCulture;
        }

    }
}
