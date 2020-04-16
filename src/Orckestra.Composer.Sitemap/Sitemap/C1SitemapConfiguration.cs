using Orckestra.Composer.Sitemap.Config;
using System;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public class C1SitemapConfiguration: IC1SitemapConfiguration
    {
        public bool IsScheduleDefined
        {
            get { return ScheduleDelayInSeconds > 0; }
        }

        public int ScheduleDelayInSeconds
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.ScheduleDelayInSeconds);                
            }
        }

        public int NumberOfEntriesPerFile
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.NumberOfEntriesPerFile);
            }
        }

        public string SitemapDirectory
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.SitemapDirectory);
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.WorkingDirectory);
            }
        }

        private static T ExtractSettingFromSitemapConfiguration<T>(Func<SitemapConfiguration, T> extractor, T defaultValue = default)
        {
            var sitemapConfig = SitemapConfiguration.Instance;

            return sitemapConfig != null ? extractor(sitemapConfig) : defaultValue;
        }
    }
}
