
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Store.Providers;
using Orckestra.Overture.ServiceModel.Customers;

namespace Composer.Store.Tests.Providers
{
    [TestFixture]
    public class StoreScheduleProvider_GetOpeningTimes
    {
        private AutoMocker _container;
        private IStoreScheduleProvider StoreScheduleProvider;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            StoreScheduleProvider = _container.CreateInstance<StoreScheduleProvider>();
        }

        [Test]
        public void WHEN_opened_all_day_SHOULD_correct_begining_and_ending_times()
        {
            //Arrange
            var schedule = BuildSchedule(null, false, true);

            //Act
            var openingTimes = StoreScheduleProvider.GetOpeningTimes(schedule, DateTime.Today);

            //Assert
            Assert.AreEqual(1, openingTimes.Count());
            Assert.AreEqual(TimeSpan.Zero, openingTimes.First().BeginingTime);
            Assert.AreEqual(TimeSpan.FromDays(1), openingTimes.First().EndingTime);
        }

        [Test]
        public void WHEN_closed_today_SHOULD_no_opening_times_for_today()
        {
            //Arrange
            var schedule = BuildSchedule(null, true, false);

            //Act
            var openingTimes = StoreScheduleProvider.GetOpeningTimes(schedule, DateTime.Today);

            //Assert
            Assert.AreEqual(0, openingTimes.Count());
        }

        [Test]
        public void WHEN_today_has_opening_times_SHOULD_be_opened()
        {
            //Arrange
            var todayOpeningTimes = new List<ScheduleInterval>
            {
                new ScheduleInterval
                {
                    BeginingTime = TimeSpan.FromHours(9),
                    EndingTime = TimeSpan.FromHours(17)
                }
            };
            var now = DateTime.Today.AddHours(13);
            var schedule = BuildSchedule(todayOpeningTimes, false, false);

            //Act
            var openingTimes = StoreScheduleProvider.GetOpeningTimes(schedule, now);

            //Assert
            var isOpenedNow = IsTimeInIntervals(now.TimeOfDay, openingTimes);
            Assert.AreEqual(true, isOpenedNow);
        }

        [Test]
        public void WHEN_recurrent_openinghourexception_SHOULD_be_closed_today()
        {
            //Arrange
            var excStartDate = new DateTime(2014, 12, 25);
            var today = new DateTime(2016, 12, 25);
            var schedule = BuildScheduleWithOpeningHourExceptions(excStartDate, excStartDate, true, today.DayOfWeek);
            //Act
            var openingTimes = StoreScheduleProvider.GetOpeningTimes(schedule, today);
            //Assert
            var isOpenedNow = IsTimeInIntervals(today.AddHours(13).TimeOfDay, openingTimes);
            Assert.AreEqual(false, isOpenedNow);
        }

        [Test]
        public void WHEN_old_openinghourexception_SHOULD_be_opened_today()
        {
            //Arrange
            var excStartDate = new DateTime(2014, 12, 25);
            var today = new DateTime(2016, 12, 25);
            var schedule = BuildScheduleWithOpeningHourExceptions(excStartDate, excStartDate, false, today.DayOfWeek);
            //Act
            var openingTimes = StoreScheduleProvider.GetOpeningTimes(schedule, today);
            //Assert
            var isOpenedNow = IsTimeInIntervals(today.AddHours(13).TimeOfDay, openingTimes);
            Assert.AreEqual(true, isOpenedNow);
        }

        /// <summary>
        /// Customer can make crossed open hours exception rules, like
        /// a) store is closed from May 1 to May 11, 2016
        /// b) store is open May 8  from 8:00 to 15:00 every year
        /// The 'Closed' rule has more priority
        /// </summary>
        [Test]
        public void WHEN_crossed_openhourexceptions_SHOULD_closed_with_more_priority()
        {
            //Arrange
            var today = DateTime.Today;

            var testDate = new DateTime(today.Year, 5, 8);
            var holidayStart = new DateTime(today.Year, 5, 1);
            var holidayEnds = new DateTime(today.Year, 5, 11);
            var schedule = BuildScheduleWithOpeningHourExceptions(holidayStart, holidayEnds, false, testDate.DayOfWeek);

            var crossedExp = new DailyScheduleException
            {
                IsClosed = false,
                IsRecurrent = true,
                StartDate = testDate,
                EndDate = testDate
            };

            schedule.OpeningHourExceptions.Add(crossedExp);

            //Act
            var openingTimes = StoreScheduleProvider.GetOpeningTimes(schedule, testDate).ToList();

            //Assert
            var isOpenedNow = IsTimeInIntervals(testDate.AddHours(13).TimeOfDay, openingTimes);
            Assert.AreEqual(false, isOpenedNow);
        }

        private FulfillmentSchedule BuildSchedule(List<ScheduleInterval> openingTimes = null, bool isClosed = false,
            bool isOpenedAllDay = true)
        {
            var schedule = new FulfillmentSchedule
            {
                OpeningHours = new List<DailySchedule>
                {
                    new DailySchedule
                    {
                        Day = DateTime.Today.DayOfWeek,
                        IsClosed = isClosed,
                        OpeningTimes = openingTimes,
                        IsOpenedAllDay = isOpenedAllDay
                    }
                }
            };
            return schedule;
        }

        private FulfillmentSchedule BuildScheduleWithOpeningHourExceptions(DateTime startDate, DateTime endDate,
            bool isRecurrent, DayOfWeek dayOfWeek)
        {
            var schedule = new FulfillmentSchedule
            {
                OpeningHourExceptions = new List<DailyScheduleException>
                {
                    new DailyScheduleException
                    {
                        IsClosed = true,
                        IsRecurrent = isRecurrent,
                        StartDate = startDate,
                        EndDate = endDate
                    }
                },
                OpeningHours = new List<DailySchedule>
                {
                    new DailySchedule
                    {
                        Day = dayOfWeek,
                        IsClosed = false,
                        IsOpenedAllDay = true
                    }
                }
            };
            return schedule;
        }

        private bool IsTimeInIntervals(TimeSpan time, IEnumerable<ScheduleInterval> intervals)
        {
            return
                intervals.Select(interval => time >= interval.BeginingTime && time < interval.EndingTime)
                    .FirstOrDefault();
        }
    }
}
