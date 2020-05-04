using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Composite.Core;
using Composite.Data;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapPageExcludeProvider : IC1ContentSitemapPageExcludeProvider
    {
        public ISiteConfiguration SiteConfiguration { get; private set; }
        private IEnumerable<Guid> _pageIdsToExcludeFromConfig = C1ContentSitemapProviderConfig.PageIdsToExclude;
        private IEnumerable<string> _siteConfigurationPagesToExcludesFromConfig = C1ContentSitemapProviderConfig.PageIdsFromConfigurationPropertiesToExclude;

        public C1ContentSitemapPageExcludeProvider(ISiteConfiguration siteConfiguration)
        {
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public virtual IEnumerable<Guid> GetPageIdsToExclude(Guid websiteId, CultureInfo culture)
        {
            var pageToExcludeIds = new List<Guid>();

            pageToExcludeIds.AddRange(_pageIdsToExcludeFromConfig);

            var pageIdsToExcludeFromTypes = GetPageIdsByProperties(websiteId, culture);
            pageToExcludeIds.AddRange(pageIdsToExcludeFromTypes);

            return pageToExcludeIds;
        }

        protected virtual IEnumerable<Guid> GetPageIdsByProperties(Guid websiteId, CultureInfo culture)
        {
            var result = new List<Guid>();
            using (var conn = new DataConnection(culture))
            {
                var byType = _siteConfigurationPagesToExcludesFromConfig.Select(r => r.Split(new[] { '|' }))
                .Select(parts => new { Type = parts[0], Prop = parts[1] })
                .GroupBy(r => r.Type);

                foreach (var group in byType)
                {
                    var type = Type.GetType(group.Key);

                    if (type == null)
                    {
                        Log.LogWarning("Sitemap generator", $"Configured {type} data type doesn't exists.");
                        continue;
                    }

                    var configData = DataFacade.GetData(type).Cast<IPageRelatedData>().FirstOrDefault(d => d.PageId == websiteId);

                    if (configData == null) continue;

                    var configDataProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(t => t.Name);

                    foreach (var propertyName in group)
                    {
                        if (configDataProperties.TryGetValue(propertyName.Prop, out PropertyInfo propertyInfo))
                        {
                            if (propertyInfo.PropertyType == typeof(Guid?))
                            {
                                var value = (Guid?)propertyInfo.GetValue(configData);
                                if (value.HasValue)
                                {
                                    yield return value.Value;
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(Guid))
                            {
                                var value = (Guid)propertyInfo.GetValue(configData);
                                if (value != Guid.Empty)
                                {
                                    yield return value;
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                var value = (string)propertyInfo.GetValue(configData);

                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    foreach (var idString in value.Split(','))
                                    {
                                        if (Guid.TryParse(idString, out Guid id))
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
    }
}
