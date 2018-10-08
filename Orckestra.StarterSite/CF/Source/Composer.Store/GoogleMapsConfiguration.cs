using System.Configuration;
using Orckestra.Composer.Configuration;

namespace Orckestra.Composer.Store
{
    public static class GoogleMapsConfiguration
    {
        static GoogleMapsConfiguration()
        {
            var config =
                ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as
                    ComposerConfigurationSection;
            var confComposer = config ?? new ComposerConfigurationSection();
            _configGoogleMap = confComposer.GoogleMapsConfiguration ?? new GoogleMapsConfigurationElement();

        }

        public static string ApiKey => _configGoogleMap.ApiKey;
        public static int MarkerPadding => _configGoogleMap.MarkerPadding;
        public static int ZoomLevel => _configGoogleMap.ZoomLevel;
        private static GoogleMapsConfigurationElement _configGoogleMap;

    }
}
