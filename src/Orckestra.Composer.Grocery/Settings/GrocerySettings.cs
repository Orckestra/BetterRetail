using Composite.Data;
using Orckestra.Composer.Services;
using System;
using System.Linq;

namespace Orckestra.Composer.Grocery.Settings
{
    public class GrocerySettings : IGrocerySettings
    {
        public IWebsiteContext WebsiteContext;
        private DataTypes.IGrocerySettingsMeta GrocerySettingsMeta;

        public GrocerySettings(IWebsiteContext websiteContext)
        {
            WebsiteContext = websiteContext;
            using (var con = new DataConnection())
            {
                GrocerySettingsMeta = con.Get<DataTypes.IGrocerySettingsMeta>().FirstOrDefault(g => g.PageId == WebsiteContext.WebsiteId);
            }
        }

        public string DefaultStore
        {
            get
            {
                return GrocerySettingsMeta?.DefaultStore ?? string.Empty;
            }
        }

        public int TimeSlotsDaysAmount
        {
            get
            {
                return GrocerySettingsMeta?.TimeSlotsDaysAmount ?? default;
            }
        }

        public TimeSpan ReservationExpirationTimeSpan
        {
            get
            {
                return TimeSpan.FromMinutes(GrocerySettingsMeta?.ReservationExpirationTime ?? default);
            }
        }
        public TimeSpan ReservationExpirationWarningTimeSpan
        {
            get
            {
                return TimeSpan.FromMinutes(GrocerySettingsMeta?.ReservationExpirationWarningTime ?? default);
            }
        }

    }
}
