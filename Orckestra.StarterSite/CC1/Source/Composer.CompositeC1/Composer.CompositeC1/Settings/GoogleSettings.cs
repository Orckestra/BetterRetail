using Composite.Data;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Services;
using System;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Settings
{
    public class GoogleSettings : IGoogleSettings
    {
        public IWebsiteContext WebsiteContext;
        private DataTypes.GoogleSettings GoogleSettingsMeta;

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
                if(GoogleSettingsMeta != null)
                {
                    return GoogleSettingsMeta.MapsApiKey;
                }

                return null;
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
    }
}
