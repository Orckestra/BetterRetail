using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public sealed class ComposerConfigurationSection : ConfigurationSection
    {
        public const string ConfigurationName = "composer/settings";

        [ConfigurationProperty(OvertureConfigurationElement.ConfigurationName, IsRequired = true)]
        public OvertureConfigurationElement OvertureConfiguration
        {
            get { return (OvertureConfigurationElement)this[OvertureConfigurationElement.ConfigurationName]; }
            set { this[OvertureConfigurationElement.ConfigurationName] = value; }
        }

        [ConfigurationProperty(SitemapConfiguration.ConfigurationName, IsRequired = false)]
        public SitemapConfiguration SitemapConfiguration
        {
            get { return (SitemapConfiguration)this[SitemapConfiguration.ConfigurationName]; }
            set { this[SitemapConfiguration.ConfigurationName] = value; }
        }

        [ConfigurationProperty(CdnDamProviderConfigurationElement.ConfigurationName)]
        public CdnDamProviderConfigurationElement CdnDamProvider
        {
            get { return (CdnDamProviderConfigurationElement)this[CdnDamProviderConfigurationElement.ConfigurationName]; }
            set { this[CdnDamProviderConfigurationElement.ConfigurationName] = value; }
        }

        [ConfigurationProperty(ComposerCookieAccesserConfigurationElement.ConfigurationName)]
        public ComposerCookieAccesserConfigurationElement ComposerCookieAccesser
        {
            get { return (ComposerCookieAccesserConfigurationElement)this[ComposerCookieAccesserConfigurationElement.ConfigurationName]; }
            set { this[ComposerCookieAccesserConfigurationElement.ConfigurationName] = value; }
        }

        [ConfigurationProperty(DefaultScopeConfigurationElement.ConfigurationName, IsRequired = false)]
        public DefaultScopeConfigurationElement DefaultScope
        {
            get { return (DefaultScopeConfigurationElement)this[DefaultScopeConfigurationElement.ConfigurationName]; }
            set { this[DefaultScopeConfigurationElement.ConfigurationName] = value; }
        }

        [ConfigurationProperty(CulturesConfigurationElement.ConfigurationName, IsRequired = false)]
        public CulturesConfigurationElement CultureConfiguration
        {
            get { return (CulturesConfigurationElement)this[CulturesConfigurationElement.ConfigurationName]; }
            set { this[CulturesConfigurationElement.ConfigurationName] = value; }
        }

        [ConfigurationProperty(InventoryConfigurationElement.ConfigurationName, IsRequired = true)]
        public InventoryConfigurationElement InventoryConfiguration
        {
            get { return (InventoryConfigurationElement)this[InventoryConfigurationElement.ConfigurationName]; }
            set { this[InventoryConfigurationElement.ConfigurationName] = value; }
		}


        [ConfigurationProperty(GoogleMapsConfigurationElement.ConfigurationName, IsRequired = false)]
        public GoogleMapsConfigurationElement GoogleMapsConfiguration
        {
            get { return (GoogleMapsConfigurationElement)this[GoogleMapsConfigurationElement.ConfigurationName]; }
            set { this[GoogleMapsConfigurationElement.ConfigurationName] = value; }
        }
    }
}
