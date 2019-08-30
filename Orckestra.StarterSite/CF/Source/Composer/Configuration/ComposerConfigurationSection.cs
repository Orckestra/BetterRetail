using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public sealed class ComposerConfigurationSection : ConfigurationSection
    {
        public const string ConfigurationName = "composer/settings";


        [ConfigurationProperty(SitemapConfiguration.ConfigurationName, IsRequired = false)]
        public SitemapConfiguration SitemapConfiguration
        {
            get { return (SitemapConfiguration)this[SitemapConfiguration.ConfigurationName]; }
            set { this[SitemapConfiguration.ConfigurationName] = value; }
        }




        [ConfigurationProperty(GoogleMapsConfigurationElement.ConfigurationName, IsRequired = false)]
        public GoogleMapsConfigurationElement GoogleMapsConfiguration
        {
            get { return (GoogleMapsConfigurationElement)this[GoogleMapsConfigurationElement.ConfigurationName]; }
            set { this[GoogleMapsConfigurationElement.ConfigurationName] = value; }
        }
    }
}
