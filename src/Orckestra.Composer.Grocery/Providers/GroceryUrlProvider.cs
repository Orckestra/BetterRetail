using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Providers
{
    public class GroceryUrlProvider : IGroceryUrlProvider
    {
        protected IPageService PageService { get; }
        protected IWebsiteContext WebsiteContext { get; }
        protected ISiteConfiguration SiteConfiguration { get; }

        public GroceryUrlProvider(ISiteConfiguration siteConfiguration, IPageService pageService, IWebsiteContext websiteContext)
        {
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public virtual string GetSearchUrl(CultureInfo cultureInfo)
        {
            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration(cultureInfo, WebsiteContext.WebsiteId);
            if (pagesConfiguration == null) return null;

            var url = PageService.GetPageUrl(pagesConfiguration.SearchPageId, cultureInfo);
            if (url == null) return null;

            return url;
        }

    }
}
