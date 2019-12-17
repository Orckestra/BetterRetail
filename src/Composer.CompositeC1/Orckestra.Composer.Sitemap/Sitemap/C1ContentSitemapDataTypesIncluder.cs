using Orckestra.Composer.Sitemap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Sitemap.Models;
using System.Globalization;
using Composite.Data;
using Composite.Core;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapDataTypesIncluder : IC1ContentSitemapDataTypesIncluder
    {
        private IEnumerable<string> _dataTypesToIncludeFromConfig = C1ContentSitemapProviderConfig.DataTypesToInclude;

        public IEnumerable<SitemapEntry> GetEntries(SitemapParams sitemapParams, CultureInfo culture)
        {
            using (var conn = new DataConnection(culture))
            {
                foreach (var typeFullName in _dataTypesToIncludeFromConfig)
                {
                    var type = Type.GetType(typeFullName);
                    if (type == null) { Log.LogInformation("Sitemap", $"The configured {type} doesn't exsits"); continue; }
                  
                    var allWebsitePages = new HashSet<Guid>(PageStructureInfo.GetAssociatedPageIds(sitemapParams.Website, SitemapScope.All));
                    var items = DataFacade.GetData(type).Cast<IPageRelatedData>().Where(d => allWebsitePages.Contains(d.PageId));

                    foreach (var item in items)
                    {
                        yield return CreateSitemapEntryFromRelatedData(sitemapParams.BaseUrl, item);
                    }
                }
            }
        }

        private SitemapEntry CreateSitemapEntryFromRelatedData(string baseUrl, IPageRelatedData relatedData)
        {
            var baseUrlWithoutEndSlash = baseUrl.TrimEnd('/');

            var dataRef = relatedData.ToDataReference();
            var pageUrlData = DataUrls.TryGetPageUrlData(dataRef);
            var url = PageUrls.BuildUrl(pageUrlData).TrimStart('/');

            return new SitemapEntry
            {
                Location = $"{baseUrlWithoutEndSlash}/{url}",
            };
        }
    }
}
