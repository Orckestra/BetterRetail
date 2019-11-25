using Orckestra.Composer.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Composite.Data;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.DataTypes;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapPageExcludeProvider
    {
        public ISiteConfiguration SiteConfiguration { get; private set; }

        public C1ContentSitemapPageExcludeProvider(ISiteConfiguration siteConfiguration)
        {
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public IEnumerable<Guid> GetPageIdsToExclude(Guid websiteId, CultureInfo culture)
        {
            //TODO: Cache
            var conf = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;
            var pageToExcludeIds = new List<Guid>();

            if (conf?.SitemapConfiguration != null)
            {
                pageToExcludeIds.AddRange(conf
                    .SitemapConfiguration
                    .ContentSitemapConfiguration
                    .PageIdsToExclude
                    .Cast<ContentSitemapPageToExcludeElement>()
                    .Select(element => new Guid(element.PageId)));

                var propertiesToExclude = conf.SitemapConfiguration
                    .ContentSitemapConfiguration
                    .TypesToExclude
                    .Cast<ContentSitemapTypeToExcludeElement>()
                    .Select(element => element.Name);

                pageToExcludeIds.AddRange(GetPageIdsByProperties(websiteId, culture, new HashSet<string>(propertiesToExclude)));
            }
            return pageToExcludeIds;
        }

        public bool PageHasToBeExcluded(Guid websiteId, Guid pageId, CultureInfo culture)
        {
            return GetPageIdsToExclude(websiteId, culture).Contains(pageId);
        }

        private IEnumerable<Guid> GetPageIdsByProperties(Guid websiteId, CultureInfo culture, HashSet<string> properties)
        {
            using (var conn = new DataConnection(culture))
            {
                var website = conn.Get<ISiteConfigurationMeta>().FirstOrDefault(d => d.PageId == websiteId);
                foreach (var propertyInfo in website?.GetType()?.GetProperties(BindingFlags.Public).Where(d => properties.Contains(d.Name)))
                {
                    if (propertyInfo.PropertyType == typeof(Guid?))
                    {
                        var value = (Guid?)propertyInfo.GetValue(website);
                        if (value.HasValue)
                        {
                            yield return value.Value;
                        }
                    } 
                    else if (propertyInfo.PropertyType == typeof(string))
                    {
                        var value = (string)propertyInfo.GetValue(website);
                       
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            foreach (var idString in value.Split(','))
                            {
                                Guid id;
                                if (Guid.TryParse(idString, out id))
                                {
                                    yield return id;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
