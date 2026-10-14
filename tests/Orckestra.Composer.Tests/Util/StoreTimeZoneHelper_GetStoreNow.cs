using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StoreTimeZoneHelper_GetStoreNow
    {
        private static readonly DateTime WinterUtcNow = new DateTime(2026, 12, 15, 16, 30, 0, DateTimeKind.Utc);

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("Invalid/Zone")]
        [TestCase("America/Vancouver")]
        [TestCase("Not A Windows Time Zone")]
        public void WHEN_time_zone_is_empty_or_invalid_SHOULD_not_throw_and_return_server_time(string timeZoneId)
        {
            //Act
            DateTime storeNow = default;
            Action action = () => storeNow = StoreTimeZoneHelper.GetStoreNow(timeZoneId, "0001", WinterUtcNow);

            //Assert
            action.ShouldNotThrow();
            storeNow.Should().Be(WinterUtcNow.ToLocalTime());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Invalid/Zone")]
        public void WHEN_time_zone_is_empty_or_invalid_SHOULD_return_null_time_zone(string timeZoneId)
        {
            //Act
            var timeZone = StoreTimeZoneHelper.GetStoreTimeZone(timeZoneId, "0001");

            //Assert
            timeZone.Should().BeNull();
        }

        [TestCase("Eastern Standard Time", 11, 30)]
        [TestCase("Pacific Standard Time", 8, 30)]
        public void WHEN_time_zone_is_valid_SHOULD_return_store_time(string timeZoneId, int expectedHour, int expectedMinute)
        {
            //Act
            var storeNow = StoreTimeZoneHelper.GetStoreNow(timeZoneId, "0001", WinterUtcNow);

            //Assert
            storeNow.Should().Be(TimeZoneInfo.ConvertTimeFromUtc(WinterUtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)));
            storeNow.TimeOfDay.Should().Be(new TimeSpan(expectedHour, expectedMinute, 0));
        }

        [Test]
        public void WHEN_time_zone_is_Yukon_Standard_Time_on_winter_date_SHOULD_return_BC_time()
        {
            //Arrange
            Assume.That(StoreTimeZoneHelper.GetStoreTimeZone("Yukon Standard Time", "0001"), Is.Not.Null,
                "'Yukon Standard Time' is not installed on this machine.");

            //Act
            var storeNow = StoreTimeZoneHelper.GetStoreNow("Yukon Standard Time", "0001", WinterUtcNow);

            //Assert
            storeNow.TimeOfDay.Should().Be(new TimeSpan(9, 30, 0));
        }
    }
}
