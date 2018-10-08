using Orckestra.Composer.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapPageExcludeProvider
    {
        public IEnumerable<Guid> GetPageIdsToExclude()
        {
            var conf = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;
            var pageToExcludeIds = Enumerable.Empty<Guid>();

            if (conf.SitemapConfiguration != null)
            {
                pageToExcludeIds = conf
                    .SitemapConfiguration
                    .ContentSitemapConfiguration
                    .PageIdsToExclude
                    .Cast<ContentSitemapPageToExcludeElement>()
                    .Select(element => new Guid(element.PageId));
            }

            return pageToExcludeIds.ToArray();
        }

        public bool PageHasToBeExcluded(Guid pageId)
        {
            return GetPageIdsToExclude().Contains(pageId);
        }
    }
}
