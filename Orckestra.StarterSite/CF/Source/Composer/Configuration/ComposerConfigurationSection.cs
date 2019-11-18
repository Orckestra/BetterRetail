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
    }
}
