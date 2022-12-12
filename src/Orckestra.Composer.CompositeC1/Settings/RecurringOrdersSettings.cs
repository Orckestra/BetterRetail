using System;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Settings
{
    public class RecurringOrdersSettings : IRecurringOrdersSettings
    {
        private readonly Lazy<DataTypes.RecurringOrdersSettings> _recurringOrdersSettingsMeta;
        private DataTypes.RecurringOrdersSettings RecurringOrdersSettingsMeta => _recurringOrdersSettingsMeta.Value;

        public RecurringOrdersSettings(IWebsiteContext websiteContext)
        {
            _recurringOrdersSettingsMeta = new Lazy<DataTypes.RecurringOrdersSettings>(websiteContext.GetRootPageMetaData<DataTypes.RecurringOrdersSettings>);
        }

        public bool Enabled => RecurringOrdersSettingsMeta?.Enabled ?? false;

        public Guid RecurringCartDetailsPageId => RecurringOrdersSettingsMeta?.RecurringCartDetailsPageId ?? Guid.Empty;

        public Guid RecurringCartsPageId => RecurringOrdersSettingsMeta?.RecurringCartsPageId ?? Guid.Empty;

        public Guid RecurringScheduleDetailsPageId => RecurringOrdersSettingsMeta?.RecurringScheduleDetailsPageId ?? Guid.Empty;

        public Guid RecurringSchedulePageId => RecurringOrdersSettingsMeta?.RecurringSchedulePageId ?? Guid.Empty;
    }
}