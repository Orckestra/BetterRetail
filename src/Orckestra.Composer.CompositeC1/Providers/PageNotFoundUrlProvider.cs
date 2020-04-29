using System;
using System.Web;
using Composite.Core;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Providers;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class PageNotFoundUrlProvider : IPageNotFoundUrlProvider
    {
        const string ErrorPathQuerystringName = "errorpath";
        protected IPageService PageService { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }

        public PageNotFoundUrlProvider(IPageService pageService, ISiteConfiguration siteConfiguration)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            SiteConfiguration = siteConfiguration;
        }

        public virtual string Get404PageUrl(string requestedPath)
        {
            if (requestedPath == null) { throw new ArgumentNullException(nameof(requestedPath)); }

            var pageUrlData = C1Helper.GetPageUrlDataFromUrl(requestedPath);
            var websiteId = C1Helper.GetWebsiteIdFromPageUrlData(pageUrlData);
            if (pageUrlData == null || websiteId == Guid.Empty) { return null; }

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(pageUrlData.LocalizationScope, websiteId);

            if (pagesConfiguration.PageNotFoundPageId != Guid.Empty)
            {
                var pageUrl = PageService.GetPageUrl(pagesConfiguration.PageNotFoundPageId, pageUrlData.LocalizationScope);

                if (pageUrl == null)
                {
                    Log.LogWarning("PageNotFoundUrlProvider", 
                        $"404 Page is not configured for website id {pageUrlData.PageId} and culture {pageUrlData.LocalizationScope}. " +
                        $"Requested path: {requestedPath}");

                    return PageService.GetPageUrl(pagesConfiguration.HomePageId, pageUrlData.LocalizationScope);
                }

                var urlBuilder = new UrlBuilder(pageUrl) { [ErrorPathQuerystringName] = HttpUtility.UrlEncode(requestedPath) };
                return urlBuilder.ToString();
            }
            else return null;    
        }
    }
}