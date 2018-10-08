using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public sealed class GoogleMapsConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "googlemaps";

        const string ApiKeyKey = "apiKey";
        const string ZoomLevelKey = "zoomlevel";
        const string MarkerPaddingKey = "markerpadding";

        [ConfigurationProperty(ApiKeyKey, IsRequired = false)]
        public string ApiKey
        {
            get { return (string) this[ApiKeyKey]; }
            set { this[ApiKeyKey] = value; }
        }

        [ConfigurationProperty(ZoomLevelKey, IsRequired = false, DefaultValue = 11)]
        public int ZoomLevel
        {
            get { return (int)this[ZoomLevelKey]; }
            set { this[ZoomLevelKey] = value; }
        }
        /// <summary>
        /// Based on marker icon size in pixels
        /// </summary>
        [ConfigurationProperty(MarkerPaddingKey, IsRequired = false, DefaultValue = 30)]
        public int MarkerPadding
        {
            get { return (int)this[MarkerPaddingKey]; }
            set { this[MarkerPaddingKey] = value; }
        }
    }
}
