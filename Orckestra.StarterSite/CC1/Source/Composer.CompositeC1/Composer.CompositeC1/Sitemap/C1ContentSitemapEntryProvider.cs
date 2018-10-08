using Orckestra.Composer.Sitemap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Sitemap.Models;
using System.Globalization;
using System.Web;
using Composite.AspNet;
using System.Diagnostics;
using System.IO;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapEntryProvider : ISitemapEntryProvider
    {
        private readonly C1ContentSitemapPageExcludeProvider _pageToExcludeProvider;

        public C1ContentSitemapEntryProvider(C1ContentSitemapPageExcludeProvider pageToExcludeProvider)
        {
            Guard.NotNull(pageToExcludeProvider, nameof(pageToExcludeProvider));

            _pageToExcludeProvider = pageToExcludeProvider;
        }

        public Task<IEnumerable<SitemapEntry>> GetEntriesAsync(string baseUrl, string scope, CultureInfo culture, int offset, int count)
        {
            // Get the instance of CompositeC1SiteMapProvider configured in the Web.config file
            //<siteMap defaultProvider="CompositeC1">
            //  <providers>
            //    <add name="CompositeC1" type="Composite.AspNet.CompositeC1SiteMapProvider, Composite" />
            //  </providers>
            //</siteMap>
            var provider = SiteMap.Provider as CmsPageSiteMapProvider;
            if (provider == null)
            {
                throw new Exception("SiteMap provider is not configured in Web.config to use Composite.AspNet.CompositeC1SiteMapProvider.");
            }

            // Because the GetRootNodes -> LoadSiteMap needs a valid HttpCurrent we create it if it is null 
            // by only using specifying the baseUrl in the HttpRequest object
            // Source: CompositeC1 -> CompositeC1SiteMapProvider.cs (line 276)
            if (HttpContext.Current == null)
            {
                HttpContext.Current = new HttpContext(
                    new HttpRequest(string.Empty, baseUrl, string.Empty),
                    new HttpResponse(new StringWriter())
                );
            }

            var rootNodes = provider.GetRootNodes().ToList();

            // Get root node associated to culture
            var rootNode = rootNodes.FirstOrDefault(node => node.Culture.Equals(culture));

            var entries = rootNode.GetAllNodes()
                .OfType<CmsPageSiteMapNode>()
                .Where(node => _pageToExcludeProvider.PageHasToBeExcluded(node.Page.Id) == false)
                .Select(node => CreateSitemapEntryFromCompositeC1SiteMapNode(baseUrl, node));

            return Task.FromResult(entries);
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
