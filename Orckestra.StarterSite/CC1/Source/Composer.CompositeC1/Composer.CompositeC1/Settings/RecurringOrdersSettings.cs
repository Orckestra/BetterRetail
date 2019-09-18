using Composite.Data;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Services;
using System;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Settings
{
    public class RecurringOrdersSettings : IRecurringOrdersSettings
    {
        public IWebsiteContext WebsiteContext;
        private DataTypes.RecurringOrdersSettings RecurringOrdersSettingsMeta;

        public RecurringOrdersSettings(IWebsiteContext websiteContext)
        {
            WebsiteContext = websiteContext;
            using(var con = new DataConnection())
            {
                RecurringOrdersSettingsMeta = con.Get<DataTypes.RecurringOrdersSettings>().FirstOrDefault(g => g.PageId == WebsiteContext.WebsiteId);
            }
        }

        public bool Enabled
        {
            get
            {
                if (RecurringOrdersSettingsMeta != null)
                {
                    return RecurringOrdersSettingsMeta.Enabled;
                }

                return false;
            }
        }

        public Guid RecurringCartDetailsPageId
        {
            get
            {
                if (RecurringOrdersSettingsMeta != null && RecurringOrdersSettingsMeta.RecurringCartDetailsPageId.HasValue)
                {
                    return RecurringOrdersSettingsMeta.RecurringCartDetailsPageId.Value;
                }

                return Guid.Empty;
            }
        }

        public Guid RecurringCartsPageId
        {
            get
            {
                if (RecurringOrdersSettingsMeta != null && RecurringOrdersSettingsMeta.RecurringCartsPageId.HasValue)
                {
                    return RecurringOrdersSettingsMeta.RecurringCartsPageId.Value;
                }

                return Guid.Empty;
            }
        }

        public Guid RecurringScheduleDetailsPageId
        {
            get
            {
                if (RecurringOrdersSettingsMeta != null && RecurringOrdersSettingsMeta.RecurringScheduleDetailsPageId.HasValue)
                {
                    return RecurringOrdersSettingsMeta.RecurringScheduleDetailsPageId.Value;
                }

                return Guid.Empty;
            }
        }

        public Guid RecurringSchedulePageId
        {
            get
            {
                if (RecurringOrdersSettingsMeta != null && RecurringOrdersSettingsMeta.RecurringSchedulePageId.HasValue)
                {
                    return RecurringOrdersSettingsMeta.RecurringSchedulePageId.Value;
                }

                return Guid.Empty;
            }
        }

    }
}
