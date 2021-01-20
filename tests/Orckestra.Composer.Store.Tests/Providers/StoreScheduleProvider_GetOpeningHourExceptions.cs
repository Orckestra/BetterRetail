using System;
using System.Collections.Generic;
using System.Linq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Store.Providers;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.Store.Tests.Providers
{
    [TestFixture]
    public class StoreScheduleProvider_GetOpeningHourExceptions
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
       public void WHEN_farfuture_openhourexception_exists_SHOULD_skip_it()
        {
            //Arrange
            var today = new DateTime(DateTime.Now.Year, 01, 01);
            var holidayStart = today.AddDays(5);
            var holidayEnds = holidayStart;
            var schedule = BuildSchedule(holidayStart, holidayEnds, true, true);

            var farFutureHoliday = new DateTime(today.Year + 3, today.Month, 1);
            var farFutureExc = new DailyScheduleException
            {
                IsClosed = true,
                IsRecurrent = false,
                StartDate = farFutureHoliday,
                EndDate = farFutureHoliday
            };

            schedule.OpeningHourExceptions.Add(farFutureExc);

            //Act
            var exceptions = StoreScheduleProvider.GetOpeningHourExceptions(schedule, today, 1).ToList();

            //Assert
            Assert.AreEqual(1 , exceptions.Count);
        }

        [Test]
        public void WHEN_openhourexception_is_recurrent_SHOULD_be_exception()
        {
            //Arrange
            var today = new DateTime(DateTime.Now.Year, 01, 01);
            var holidayStart = new DateTime(today.Year - 1, today.AddDays(5).Month, today.AddDays(5).Day);
            var holidayEnds = holidayStart;
            var schedule = BuildSchedule(holidayStart, holidayEnds, true, true);
            //Act
            var exceptions = StoreScheduleProvider.GetOpeningHourExceptions(schedule, today, 1).ToList();
            //Assert
            Assert.AreEqual(1, exceptions.Count);
            Assert.AreEqual(exceptions[0].StartDate.Year, today.Year);
        }

        [Test]
        public void WHEN_openhourexception_is_not_recurrent_SHOULD_Be_no_exceptions()
        {
            //Arrange
            var today = new DateTime(DateTime.Now.Year, 01, 01);
            var holidayStart = new DateTime(today.Year - 1, today.AddDays(5).Month, today.AddDays(5).Day);
            var holidayEnds = holidayStart;
            var schedule = BuildSchedule(holidayStart, holidayEnds, false, true);
            //Act
            var exceptions = StoreScheduleProvider.GetOpeningHourExceptions(schedule, today, 1).ToList();
            //Assert
            Assert.AreEqual(0, exceptions.Count);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WHEN_openhourexception_is_today_SHOULD_be_returned(bool isRecurrent)
        {
            //Arrange
            var today = new DateTime(DateTime.Now.Year, 01, 01);
            var schedule = BuildSchedule(today, today, isRecurrent, true);
            //Act
            var exceptions = StoreScheduleProvider.GetOpeningHourExceptions(schedule, today, 1).ToList();
            //Assert
            Assert.AreEqual(1, exceptions.Count);
        }


        private FulfillmentSchedule BuildSchedule(DateTime startDate, DateTime endDate,
            bool isRecurrent, bool isClosed)
        {
            var schedule = new FulfillmentSchedule
            {
                OpeningHourExceptions = new List<DailyScheduleException>
                {
                    new DailyScheduleException
                    {
                        IsClosed = isClosed,
                        IsRecurrent = isRecurrent,
                        StartDate = startDate,
                        EndDate = endDate
                    }
                }
            };
            return schedule;
        }
    }
}
