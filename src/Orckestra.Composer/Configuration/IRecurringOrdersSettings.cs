using System;

namespace Orckestra.Composer.Configuration
{
    public interface IRecurringOrdersSettings
    {
        bool Enabled { get;  }
        Guid RecurringSchedulePageId { get;  }
        Guid RecurringScheduleDetailsPageId { get; }
        Guid RecurringCartDetailsPageId { get; }
        Guid RecurringCartsPageId { get; }
    }
}
