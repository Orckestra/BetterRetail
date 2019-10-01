using System;
using System.Globalization;
using System.Linq;
using System.Web;
using Composite.Core;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Composer.Services;
using Composite.Core.Routing;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class PageNotFoundUrlProvider : IPageNotFoundUrlProvider
    {
        const string ErrorPathQuerystringName = "errorpath";

        protected IPageService PageService { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }

        public PageNotFoundUrlProvider(IPageService pageService,
            ISiteConfiguration siteConfiguration)
        {
            if (pageService == null)
            {
                throw new ArgumentNullException(nameof(pageService));
            }

            PageService = pageService;
            SiteConfiguration = siteConfiguration;
        }

        public virtual string Get404PageUrl(string requestedPath)
        {
            if (requestedPath == null)
            {
                throw new ArgumentException("Requested Path is required", nameof(requestedPath));
            }

            string url = requestedPath;
            PageUrlData pageUrlData = null;
            while (pageUrlData == null && url.LastIndexOf('/') > 0)
            {
                url = url.Substring(0, url.LastIndexOf('/'));
                pageUrlData = PageUrls.ParseUrl(url.ToString());
            }
            if (pageUrlData == null)
            {
                return null;
            }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(pageUrlData.LocalizationScope, pageUrlData.PageId);
            if (pagesConfiguration.PageNotFoundPageId != Guid.Empty)
            {
                var pageUrl = PageService.GetPageUrl(pagesConfiguration.PageNotFoundPageId, pageUrlData.LocalizationScope);
                var urlBuilder = new UrlBuilder(pageUrl) { [ErrorPathQuerystringName] = HttpUtility.UrlEncode(requestedPath) };

                return urlBuilder.ToString();
            }
            else return null;
          
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
