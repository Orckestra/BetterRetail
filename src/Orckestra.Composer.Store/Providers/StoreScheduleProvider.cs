using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.Store.Providers
{
    public class StoreScheduleProvider : IStoreScheduleProvider
    {
        public virtual IEnumerable<ScheduleInterval> GetOpeningTimes(FulfillmentSchedule schedule, DateTime dateTime)
        {
            var openingHourExceptions = schedule.OpeningHourExceptions.Where(dse =>
                dse.IsRecurrent
                    ? IsDateInRecurrentRange(dateTime, dse.StartDate, dse.EndDate)
                    : IsDateInRange(dateTime, dse.StartDate, dse.EndDate)
                ).ToList();

            if (openingHourExceptions.Any(ohe => ohe.IsClosed))
            {
                yield break;
            }
            if (openingHourExceptions.Any())
            {
                foreach (var openHourException in openingHourExceptions)
                {
                    yield return openHourException.OpeningTime;
                }
                yield break;
            }

            // Scheduler
            foreach (var openHours in schedule.OpeningHours.Where(oh => oh.Day == dateTime.DayOfWeek))
            {
                if (openHours.IsOpenedAllDay)
                {
                    yield return new ScheduleInterval()
                    {
                        BeginingTime = TimeSpan.Zero,
                        EndingTime = TimeSpan.FromDays(1)
                    };
                    yield break;
                }
                if (openHours.IsClosed)
                {
                    yield break;
                }
                foreach (var openingTime in openHours.OpeningTimes)
                {
                    yield return openingTime;
                }
            }
        }

        public virtual IEnumerable<DailyScheduleException> GetOpeningHourExceptions(FulfillmentSchedule schedule,
            DateTime dateTime,
            int periodInYears = 1)
        {
            var exceptions = schedule.OpeningHourExceptions
                .Where(
                    ex => ex.IsRecurrent
                        ? ex.StartDate.DayOfYear >= dateTime.DayOfYear
                        : IsDateInRange(ex.StartDate, dateTime, dateTime.AddYears(periodInYears)))
                .OrderBy(ex => ex.StartDate.DayOfYear);

            foreach (var ex in exceptions)
            {
                if (ex.IsRecurrent)
                {
                    var dateNow = DateTime.Now;
                    ex.StartDate = new DateTime(dateNow.Year, ex.StartDate.Month, ex.StartDate.Day);
                    ex.EndDate = new DateTime(dateNow.Year, ex.EndDate.Month, ex.EndDate.Day);
                }
            }
            return exceptions.OrderBy(ex => ex.StartDate.Year);
        }

        protected virtual bool IsDateInRecurrentRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            //If 29 February converted 28 February then set it to March 1 for start date (.AddDays(-1)...AddDays(1))
            var currentStartDate = startDate.Date.AddDays(-1).AddYears(date.Year - startDate.Year).AddDays(1);
            var currentEndDate = endDate.Date.AddYears(date.Year - endDate.Year);

            currentEndDate = currentEndDate < startDate.Date.AddYears(date.Year - startDate.Year) ? currentEndDate.AddYears(1) : currentEndDate;
            return IsDateInRange(date, currentStartDate, currentEndDate);
        }

        protected virtual bool IsDateInRange(DateTime dateTime, DateTime startDate, DateTime endDate)
        {
            return dateTime.Date >= startDate.Date && dateTime.Date <= endDate.Date;
        }
    }
}
