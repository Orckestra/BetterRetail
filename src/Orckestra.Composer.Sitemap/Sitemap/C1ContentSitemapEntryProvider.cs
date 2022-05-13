using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Composite.AspNet;
using Orckestra.Composer.Sitemap;
using Orckestra.Composer.Sitemap.Models;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapEntryProvider : ISitemapEntryProvider
    {
        private readonly IC1ContentSitemapPageExcludeProvider _pageToExcludeProvider;
        private readonly IC1ContentSitemapDataTypesIncluder _dynamicPagesEntryProvider;

        public C1ContentSitemapEntryProvider(IC1ContentSitemapPageExcludeProvider pageToExcludeProvider, IC1ContentSitemapDataTypesIncluder dynamicPagesEntryProvider)
        {
            _pageToExcludeProvider = pageToExcludeProvider ?? throw new ArgumentNullException(nameof(pageToExcludeProvider));
            _dynamicPagesEntryProvider = dynamicPagesEntryProvider ?? throw new ArgumentNullException(nameof(dynamicPagesEntryProvider));
        }

        public Task<IEnumerable<SitemapEntry>> GetEntriesAsync(SitemapParams sitemapParams, CultureInfo culture, int offset, int count)
        {
            // Get the instance of CompositeC1SiteMapProvider configured in the Web.config file
            //<siteMap defaultProvider="CompositeC1">
            //  <providers>
            //    <add name="CompositeC1" type="Composite.AspNet.CompositeC1SiteMapProvider, Composite" />
            //  </providers>
            //</siteMap>
            if (!(SiteMap.Provider is CmsPageSiteMapProvider provider))
            {
                throw new Exception("SiteMap provider is not configured in Web.config to use Composite.AspNet.CompositeC1SiteMapProvider.");
            }

            // Because the GetRootNodes -> LoadSiteMap needs a valid HttpCurrent we create it if it is null 
            // by only using specifying the baseUrl in the HttpRequest object
            // Source: CompositeC1 -> CompositeC1SiteMapProvider.cs (line 276)
            RequestUtils.DefineHttpContextIfNotExist(sitemapParams.BaseUrl);

            var entriesList = new List<SitemapEntry>();

            var rootNodes = provider.GetRootNodes().ToList();

            // Get root node associated to culture and website
            var rootNode = rootNodes.Find(node => node.Culture.Equals(culture) && node.Page?.Id == sitemapParams.Website);

            var pageIdsToExclude = _pageToExcludeProvider.GetPageIdsToExclude(sitemapParams.Website, culture);
            var allNodes = rootNode?.GetAllNodes()
                .OfType<CmsPageSiteMapNode>()
                .Where(node => node.Page == null || !pageIdsToExclude.Contains(node.Page.Id))
                .Select(node => CreateSitemapEntryFromCompositeC1SiteMapNode(sitemapParams.BaseUrl, node));
            entriesList.AddRange(allNodes);

            var dynamicTypesToInclude = _dynamicPagesEntryProvider.GetEntries(sitemapParams, culture);
            entriesList.AddRange(dynamicTypesToInclude);
            return Task.FromResult(entriesList as IEnumerable<SitemapEntry>);
        }

        private SitemapEntry CreateSitemapEntryFromCompositeC1SiteMapNode(string baseUrl, CmsPageSiteMapNode node)
        {
            var baseUrlWithoutEndSlash = baseUrl.TrimEnd('/');
            var nodeUrl = node.Url.TrimStart('/');

            var sitemapEntry = new SitemapEntry
            {
                Location = $"{baseUrlWithoutEndSlash}/{nodeUrl}",
                LastModification = node.LastModified.ToUniversalTime().ToString("u").Replace(" ", "T"),
            };

            if (node.Priority.HasValue)
            {
                sitemapEntry.Priority = ((decimal)node.Priority.Value / 10).ToString("0.0", CultureInfo.InvariantCulture);
            }

            if (node.ChangeFrequency.HasValue)
            {
                sitemapEntry.ChangeFrequency = node.ChangeFrequency.Value.ToString().ToLowerInvariant();
            }

            return sitemapEntry;
        }
    }
}
