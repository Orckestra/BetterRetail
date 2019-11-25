using System;
using System.Configuration;
using System.Linq;

namespace Orckestra.Composer.Configuration
{
    public class SitemapConfiguration : ConfigurationElement
    {
        internal const string ConfigurationName = "sitemap";

        const string JobCronScheduleKey = "jobCronSchedule";
        [ConfigurationProperty(JobCronScheduleKey, IsRequired = true)]        
        public string JobCronSchedule
        {
            get
            {
                var settingValue = this[JobCronScheduleKey] as string;
                if (string.IsNullOrWhiteSpace(settingValue))
                {
                    return null;
                }

                return settingValue;
            }
            set
            {
                this[JobCronScheduleKey] = value;
            }
        }

        const string BaseUrlKey = "baseUrl";
        [ConfigurationProperty(BaseUrlKey, IsRequired = true)]
        public string BaseUrl
        {
            get
            {
                return GetSettingValueThrowsIfNullOrWhitespace(BaseUrlKey);
            }
            set
            {
                this[BaseUrlKey] = value;
            }
        }

        const string SitemapDirectoryKey = "sitemapDirectory";
        [ConfigurationProperty(SitemapDirectoryKey, IsRequired = true)]
        public string SitemapDirectory
        {
            get
            {
                return GetSettingValueThrowsIfNullOrWhitespace(SitemapDirectoryKey);
            }
            set
            {
                this[SitemapDirectoryKey] = value;
            }
        }

        const string WorkingDirectoryKey = "workingDirectory";
        [ConfigurationProperty(WorkingDirectoryKey, IsRequired = true)]
        public string WorkingDirectory
        {
            get
            {
                return GetSettingValueThrowsIfNullOrWhitespace(WorkingDirectoryKey);
            }
            set
            {
                this[WorkingDirectoryKey] = value;
            }
        }

        [ConfigurationProperty(ProductSitemapConfiguration.ConfigurationName, IsRequired = false)]
        public ProductSitemapConfiguration ProductSitemapConfiguration
        {
            get { return (ProductSitemapConfiguration)this[ProductSitemapConfiguration.ConfigurationName]; }
            set { this[ProductSitemapConfiguration.ConfigurationName] = value; }
        }

        [ConfigurationProperty(ContentSitemapConfiguration.ConfigurationName, IsRequired = false)]
        public ContentSitemapConfiguration ContentSitemapConfiguration
        {
            get { return (ContentSitemapConfiguration)this[ContentSitemapConfiguration.ConfigurationName]; }
            set { this[ContentSitemapConfiguration.ConfigurationName] = value; }
        }

        private Exception CreateConfigurationErrorsException(string settingName, object settingValue)
        {
            return new ConfigurationErrorsException(
                $"The value ('{settingValue}') of the '{settingName}' property defined in the 'sitemap' element in the 'composer' configuration is invalid."
            );
        }

        private string GetSettingValueThrowsIfNullOrWhitespace(string key)
        {
            string settingValue = this[key] as string;
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                throw CreateConfigurationErrorsException(key, settingValue);
            }

            return settingValue;
        }
    }
}
