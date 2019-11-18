using Orckestra.Composer.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public static class C1SitemapConfiguration
    {
        public static bool IsScheduleDefined
        {
            get { return JobCronSchedule != null; }
        }

        public static string JobCronSchedule
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.JobCronSchedule);                
            }
        }

        public static string BaseUrl
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.BaseUrl);
            }
        }

        public static string SitemapDirectory
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.SitemapDirectory);
            }
        }

        public static string WorkingDirectory
        {
            get
            {
                return ExtractSettingFromSitemapConfiguration((config) => config.WorkingDirectory);
            }
        }

        private static T ExtractSettingFromSitemapConfiguration<T>(Func<SitemapConfiguration, T> extractor)
        {
            var conf = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;
            T settingValue = default(T);

            if (conf.SitemapConfiguration != null)
            {
                settingValue = extractor(conf.SitemapConfiguration);
            }

            return settingValue;
        }
    }
}
