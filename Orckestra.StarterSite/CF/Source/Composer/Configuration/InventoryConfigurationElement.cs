using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public class InventoryConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "inventory";

        [ConfigurationProperty(DefaultInventoryAndFulfillmentLocationIdKey, IsRequired = true)]
        public string DefaultInventoryAndFulfillmentLocationId
        {
            get { return (string)this[DefaultInventoryAndFulfillmentLocationIdKey]; }
            set { this[DefaultInventoryAndFulfillmentLocationIdKey] = value; }
        }
        private const string DefaultInventoryAndFulfillmentLocationIdKey = "locationId";
    }
}
