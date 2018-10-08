using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public class CulturesConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "cultures";

        [ConfigurationProperty(SupportedCulturesKey, IsRequired = true)]
        public string SupportedCultures
        {
            get { return (string)this[SupportedCulturesKey]; }
            set { this[SupportedCulturesKey] = value; }
        }
        private const string SupportedCulturesKey = "supported";


        [ConfigurationProperty(DefaultCultureKey, IsRequired = true)]
        public string DefaultCulture
        {
            get { return (string)this[DefaultCultureKey]; }
            set { this[DefaultCultureKey] = value; }
        }
        private const string DefaultCultureKey = "default";
    }
}
