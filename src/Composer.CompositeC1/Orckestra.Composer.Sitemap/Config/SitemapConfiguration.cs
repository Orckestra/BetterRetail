using System;
using System.Configuration;

namespace Orckestra.Composer.Sitemap.Config
{
    public class SitemapConfiguration : ConfigurationSection
    {
        public const string ConfigurationName = "composer/sitemap";
        public static SitemapConfiguration Instance => ConfigurationManager.GetSection(ConfigurationName) as SitemapConfiguration;

        const string ScheduleDelayInSecondsKey = "scheduleDelayInSeconds";
        [ConfigurationProperty(ScheduleDelayInSecondsKey, IsRequired = true)]
        public int ScheduleDelayInSeconds
        {
            get
            {
                return (int)this[ScheduleDelayInSecondsKey];
            }
            set
            {
                this[ScheduleDelayInSecondsKey] = value;
            }
        }

        const string NumberOfEntriesPerFileKey = "numberOfEntriesPerFile";
        [ConfigurationProperty(NumberOfEntriesPerFileKey, IsRequired = true)]
        [IntegerValidator]
        public virtual int NumberOfEntriesPerFile
        {
            get { return (int)this[NumberOfEntriesPerFileKey]; }
            set { this[NumberOfEntriesPerFileKey] = value; }
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
