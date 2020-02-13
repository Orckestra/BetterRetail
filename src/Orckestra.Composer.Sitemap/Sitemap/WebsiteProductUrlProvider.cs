using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class WebsiteProductUrlProvider : IProductUrlProvider
    {
        public IPageService PageService { get; }
        public ISiteConfiguration SiteConfiguration { get; }

        public WebsiteProductUrlProvider(IPageService pageService, ISiteConfiguration siteConfiguration)
        {
            PageService = pageService;
            SiteConfiguration = siteConfiguration;
        }
        public string GetProductUrl(GetProductUrlParam parameters)
        {
            if(parameters is WebsiteGetProductUrlParam websitesParameters)
            {
                var provider = new ProductUrlProvider(PageService,
                    new Sitemap.WebsiteContextWrapper(websitesParameters.WebsiteId), SiteConfiguration);
                return provider.GetProductUrl(parameters);
            }

            return null;
        }

        public void RegisterRoutes(RouteCollection routeCollection)
        {
        }
    }
}
