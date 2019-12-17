using Orckestra.Composer.Sitemap.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1ContentSitemapProviderConfig : ISitemapProviderConfig
    {
        private const string Default_SitemapFilePrefix = "content";

        public string SitemapFilePrefix
        {
            get
            {
                return ExtractSettingFromContentSitemapConfiguration((config) => config.SitemapFilePrefix, Default_SitemapFilePrefix);
            }
        }

        public static IEnumerable<Guid> PageIdsToExclude
        {
            get
            {
                return ExtractSettingFromContentSitemapConfiguration((config) => config.PageIdsToExclude)
                    .Cast<ContentSitemapPageToExcludeElement>()
                    .Select(element => new Guid(element.PageId));
            }
        }

        public static IEnumerable<string> PageIdsFromConfigurationPropertiesToExclude
        {
            get
            {
                return ExtractSettingFromContentSitemapConfiguration((config) => config.PageIdsFromConfigurationPropertiesToExclude)
                    .Cast<PageTypeFromConfigurationPropertiesElement>()
                    .Select(element => element.Name); ;
            }
        }

        public static IEnumerable<string> DataTypesToInclude
        {
            get
            {
                return ExtractSettingFromContentSitemapConfiguration((config) => config.DataTypesToInclude)
                    .Cast<PageTypeFromConfigurationPropertiesElement>()
                    .Select(element => element.Name); ;
            }
        }

        private static T ExtractSettingFromContentSitemapConfiguration<T>(Func<ContentSitemapConfiguration, T> extractor, T defaultValue = default)
        {
            var contentSitemapConfiguration = SitemapConfiguration.Instance?.ContentSitemapConfiguration;

            return contentSitemapConfiguration != null ? extractor(contentSitemapConfiguration) : defaultValue;
        }
    }
}
