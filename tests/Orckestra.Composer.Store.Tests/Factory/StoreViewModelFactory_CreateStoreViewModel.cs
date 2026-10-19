using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Store.Factory;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Store.Tests.Factory
{
    [TestFixture]
    public class StoreViewModelFactory_GetStoreViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(CreateStoreUrlProvider());
        }

        [TestCase("1234567890", "(123) 456-7890")]
        public void WHEN_PhoneNumber_has_formating_SHOULD_be_formated(string phoneNumber, string formatedPhoneNumber)
        {
            //Arrange
            var store = CreateStoreObject();
            store.FulfillmentLocation.Addresses.First().PhoneNumber = phoneNumber;
            _container.Use(CreateViewModelMapper());
            _container.Use(CreateLocalizationProvider("PhoneNumberFormat", "{0:(###) ###-####}"));

            var storeViewModelFactory = _container.CreateInstance<StoreViewModelFactory>();

            //Act
            var model = storeViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
            {
                Store = store,
                BaseUrl = "baseUrl",
                CultureInfo = CultureInfo.CreateSpecificCulture("en-CA")
            });

            //Assert
            model.Address.PhoneNumber.ShouldBeEquivalentTo(formatedPhoneNumber);
        }

        [TestCase("Invalid/Zone")]
        [TestCase("")]
        [TestCase(null)]
        public void WHEN_TimeZone_is_invalid_or_empty_SHOULD_create_store_with_schedule(string timeZone)
        {
            //Arrange
            var store = CreateStoreObjectWithSchedule(timeZone, new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59));
            _container.Use(CreateViewModelMapper());
            _container.Use(CreateStoreScheduleProvider(store.StoreSchedule));

            var storeViewModelFactory = _container.CreateInstance<StoreViewModelFactory>();

            //Act
            StoreViewModel model = null;
            Action action = () => model = storeViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
            {
                Store = store,
                BaseUrl = "baseUrl",
                CultureInfo = CultureInfo.CreateSpecificCulture("en-CA")
            });

            //Assert
            action.ShouldNotThrow();
            model.Schedule.Should().NotBeNull();
            model.Schedule.OpeningHours.Should().HaveCount(7);
            model.Schedule.TodayOpeningTimes.Should().HaveCount(1);
        }

        [TestCase(16, 30, true, Description = "09:30 in BC")]
        [TestCase(15, 30, false, Description = "08:30 in BC")]
        [TestCase(1, 0, false, Description = "18:00 in BC, previous day")]
        public void WHEN_TimeZone_is_Yukon_Standard_Time_SHOULD_compute_open_now_in_store_time(int utcHour, int utcMinute, bool expectedIsOpenNow)
        {
            //Arrange
            Assume.That(TimeZoneInfo.GetSystemTimeZones().Any(tz => tz.Id == "Yukon Standard Time"), Is.True,
                "'Yukon Standard Time' is not installed on this machine.");

            var store = CreateStoreObjectWithSchedule("Yukon Standard Time", new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0));
            _container.Use(CreateViewModelMapper());
            _container.Use(CreateStoreScheduleProvider(store.StoreSchedule));

            var storeViewModelFactory = _container.CreateInstance<FixedUtcNowStoreViewModelFactory>();
            storeViewModelFactory.UtcNow = new DateTime(2026, 12, 15, utcHour, utcMinute, 0, DateTimeKind.Utc);

            //Act
            var model = storeViewModelFactory.CreateStoreViewModel(new CreateStoreViewModelParam
            {
                Store = store,
                BaseUrl = "baseUrl",
                CultureInfo = CultureInfo.CreateSpecificCulture("en-CA")
            });

            //Assert
            model.Schedule.IsOpenNow.Should().Be(expectedIsOpenNow);
        }

        public class FixedUtcNowStoreViewModelFactory : StoreViewModelFactory
        {
            public DateTime UtcNow { get; set; }

            public FixedUtcNowStoreViewModelFactory(
                ILocalizationProvider localizationProvider,
                IStoreUrlProvider storeUrlProvider,
                IViewModelMapper viewModelMapper,
                IStoreScheduleProvider storeScheduleProvider,
                Orckestra.Composer.Country.ICountryService countryService,
                IGoogleMapsUrlProvider googleMapsUrlProvider)
                : base(localizationProvider, storeUrlProvider, viewModelMapper, storeScheduleProvider, countryService, googleMapsUrlProvider)
            {
            }

            protected override DateTime GetStoreNowTime(CreateStoreViewModelParam param)
            {
                return Orckestra.Composer.Utils.StoreTimeZoneHelper.GetStoreNow(param.Store.FulfillmentLocation.TimeZone, param.Store.Number, UtcNow);
            }
        }

        private static Mock<IStoreScheduleProvider> CreateStoreScheduleProvider(FulfillmentSchedule schedule)
        {
            var provider = new Mock<IStoreScheduleProvider>();

            provider.Setup(p => p.GetOpeningTimes(It.IsAny<FulfillmentSchedule>(), It.IsAny<DateTime>()))
                .Returns((FulfillmentSchedule s, DateTime date) => schedule.OpeningHours.First(oh => oh.Day == date.DayOfWeek).OpeningTimes);

            provider.Setup(p => p.GetOpeningHourExceptions(It.IsAny<FulfillmentSchedule>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(new List<DailyScheduleException>());

            return provider;
        }

        private static Orckestra.Overture.ServiceModel.Customers.Stores.Store CreateStoreObjectWithSchedule(string timeZone, TimeSpan opens, TimeSpan closes)
        {
            var store = CreateStoreObject();
            store.FulfillmentLocation.TimeZone = timeZone;
            store.StoreSchedule = new FulfillmentSchedule
            {
                OpeningHours = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>()
                    .Select(day => new DailySchedule
                    {
                        Day = day,
                        OpeningTimes = new List<ScheduleInterval>
                        {
                            new ScheduleInterval { BeginingTime = opens, EndingTime = closes }
                        }
                    }).ToList()
            };

            return store;
        }


        private Mock<IStoreUrlProvider> CreateStoreUrlProvider()
        {
            Mock<IStoreUrlProvider> storeUrlProvider = new Mock<IStoreUrlProvider>();

            storeUrlProvider.Setup(p => p.GetStoreUrl(It.IsAny<GetStoreUrlParam>()))
                .Returns(() => GetRandom.String(128))
                .Verifiable();

            return storeUrlProvider;
        }

        private Mock<ILocalizationProvider> CreateLocalizationProvider(string key, string resultLocalizedString)
        {
            Mock<ILocalizationProvider> provider = new Mock<ILocalizationProvider>();

            provider.Setup(p => p.GetLocalizedString(It.Is<GetLocalizedParam>(lp => lp.Key == key)))
                .Returns(() => resultLocalizedString)
                .Verifiable();

            return provider;
        }

        private static Orckestra.Overture.ServiceModel.Customers.Stores.Store CreateStoreObject()
        {
            var store = new Orckestra.Overture.ServiceModel.Customers.Stores.Store
            {
                Number = "0001",
                FulfillmentLocation = new FulfillmentLocation
                {
                    Id = new Guid(),
                    Addresses = new List<Address>
                    {
                        new Address()
                    }
                }
            };

            return store;
        }

        private static Mock<IViewModelMapper> CreateViewModelMapper()
        {
            Mock<IViewModelMapper> mapperMock = new Mock<IViewModelMapper>(MockBehavior.Strict);

            mapperMock.Setup(
                mapper =>
                    mapper.MapTo<StoreViewModel>(It.IsAny<Orckestra.Overture.ServiceModel.Customers.Stores.Store>(),
                        It.IsAny<CultureInfo>()))
                .Returns(new StoreViewModel())
                .Verifiable();

            mapperMock.Setup(
              mapper =>
                  mapper.MapTo<StoreAddressViewModel>(It.IsAny<Orckestra.Overture.ServiceModel.Address>(),
                      It.IsAny<CultureInfo>()))
              .Returns(new StoreAddressViewModel())
              .Verifiable();

            return mapperMock;
        }

    }
}
