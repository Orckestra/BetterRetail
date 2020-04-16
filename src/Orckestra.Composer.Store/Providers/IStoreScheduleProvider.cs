using System;
using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.Store.Providers
{
    public interface IStoreScheduleProvider
    {
        IEnumerable<ScheduleInterval> GetOpeningTimes(FulfillmentSchedule schedule, DateTime dateTime);
        IEnumerable<DailyScheduleException> GetOpeningHourExceptions(FulfillmentSchedule schedule, DateTime dateTime, int periodInYears);
    }
}
