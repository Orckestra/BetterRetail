using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Settings
{
    public class GoogleSettings : IGoogleSettings
    {
        private readonly Lazy<DataTypes.GoogleSettings> _googleSettingsMeta;
        private DataTypes.GoogleSettings GoogleSettingsMeta => _googleSettingsMeta.Value;

        private readonly string _mapsApiKeyAppSettingName = "GoogleSettings.MapsApiKey";

        public GoogleSettings(IWebsiteContext websiteContext)
        {
            _googleSettingsMeta = new Lazy<DataTypes.GoogleSettings>(websiteContext.GetRootPageMetaData<DataTypes.GoogleSettings>);
        }

        public string GTMContainerId => GoogleSettingsMeta?.GTMContainerId;

        public string MapsApiKey
        {
            get
            {
                var apiKey = GoogleSettingsMeta?.MapsApiKey;
                return !string.IsNullOrWhiteSpace(apiKey) ? apiKey : ConfigurationManager.AppSettings[_mapsApiKeyAppSettingName];
            }
        }

        public int MapsZoomLevel => GoogleSettingsMeta?.MapsZoomLevel ?? 11;

        public int MapsMarkerPadding => GoogleSettingsMeta?.MapsMarkerPadding ?? 30;

        public LengthMeasureUnitEnum LengthMeasureUnit => (LengthMeasureUnitEnum)Enum.Parse(typeof(LengthMeasureUnitEnum), GoogleSettingsMeta.LengthMeasureUnit);

        public decimal? StoresAvailabilityDistance => GoogleSettingsMeta.StoresAvailabilityDistance;

        public static IEnumerable GetAvailableLengthMeasureUnits()
        {
            IEnumerable<LengthMeasureUnitEnum> values = Enum.GetValues(typeof(LengthMeasureUnitEnum)).Cast<LengthMeasureUnitEnum>();
            return values.Select(d => new { Key = d, Label = LocalizationHelper.LocalizedFormat("General", d.ToString(), d, CultureInfo.CurrentUICulture) });
        }
    }
}
