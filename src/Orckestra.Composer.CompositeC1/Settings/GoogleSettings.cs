using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Composite.Data;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Settings
{
    public class GoogleSettings : IGoogleSettings
    {
        public IWebsiteContext WebsiteContext;
        private DataTypes.GoogleSettings GoogleSettingsMeta;

        private readonly string _mapsApiKeyAppSettingName = "GoogleSettings.MapsApiKey";

        public GoogleSettings(IWebsiteContext websiteContext)
        {
            WebsiteContext = websiteContext;
            using(var con = new DataConnection())
            {
                GoogleSettingsMeta = con.Get<DataTypes.GoogleSettings>().FirstOrDefault(g => g.PageId == WebsiteContext.WebsiteId);
            }
        }

        public string GTMContainerId
        {
            get
            {
                if (GoogleSettingsMeta != null)
                {
                    return GoogleSettingsMeta.GTMContainerId;
                }

                return null;
            }
        }
        public string MapsApiKey
        {
            get
            {
                if(GoogleSettingsMeta != null && !string.IsNullOrWhiteSpace(GoogleSettingsMeta.MapsApiKey))
                {
                    return GoogleSettingsMeta.MapsApiKey;
                }

                return ConfigurationManager.AppSettings[_mapsApiKeyAppSettingName];
            }
        }
        public int MapsZoomLevel
        {
            get
            {
                if (GoogleSettingsMeta != null && GoogleSettingsMeta.MapsZoomLevel.HasValue)
                {
                    return GoogleSettingsMeta.MapsZoomLevel.Value;
                }

                return 11;
            }
        }
        public int MapsMarkerPadding
        {
            get
            {
                if (GoogleSettingsMeta != null && GoogleSettingsMeta.MapsMarkerPadding.HasValue)
                {
                    return GoogleSettingsMeta.MapsMarkerPadding.Value;
                }

                return 30;
            }
        }
        public LengthMeasureUnitEnum LengthMeasureUnit
        {
            get
            {
                return (LengthMeasureUnitEnum)Enum.Parse(typeof(LengthMeasureUnitEnum), GoogleSettingsMeta.LengthMeasureUnit);
            }
        }

        public decimal? StoresAvailabilityDistance
        {
            get
            {
                return GoogleSettingsMeta.StoresAvailabilityDistance;
            }
        }

        public static IEnumerable GetAvailableLengthMeasureUnits()
        {
            IEnumerable<LengthMeasureUnitEnum> values = Enum.GetValues(typeof(LengthMeasureUnitEnum)).Cast<LengthMeasureUnitEnum>();
            return values.Select(d => new { Key = d, Label = LocalizationHelper.LocalizedFormat("General", d.ToString(), d, CultureInfo.CurrentUICulture) });
        }
    }
}
