using System;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers.RegionCode;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Tests.Providers.RegionCode
{
    [TestFixture]
    public class RegionCodeProviderGetRegion
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [TearDown]
        public void TearDown()
        {
            _container.VerifyAll();
        }

        [Test]
        public void WHEN_Passing_Invalid_Country_SHOULD_Throw_ArgumentException()
        {
            //Arrange
            var regionProvider = new RegionCodeProvider();

            // Act
            Action action = () => regionProvider.GetRegion("12345", "IR");

            // Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestCase("A5A0G5", "CA")]
        [TestCase("B5A0G5", "CA")]
        [TestCase("C5A0G5", "CA")]
        [TestCase("E5A0G5", "CA")]
        [TestCase("G5A0G5", "CA")]
        [TestCase("H5A0G5", "CA")]
        [TestCase("J5A0G5", "CA")]
        [TestCase("K5A0G5", "CA")]
        [TestCase("L5A0G5", "CA")]
        [TestCase("M5A0G5", "CA")]
        [TestCase("N5A0G5", "CA")]
        [TestCase("P5A0G5", "CA")]
        [TestCase("R5A0G5", "CA")]
        [TestCase("S5A0G5", "CA")]
        [TestCase("T5A0G5", "CA")]
        [TestCase("V5A0G5", "CA")]
        [TestCase("X5A0G5", "CA")]
        [TestCase("Y5A0G5", "CA")]
        public void WHEN_Passing_Valid_Parameters_Canada_Country_SHOULD_Succeed(string postalCode, string countryCode)
        {
            //Arrange
            var regionProvider = new RegionCodeProvider();

            // Act
            Action action = () => regionProvider.GetRegion(postalCode, countryCode);

            // Assert
            action.ShouldNotThrow();
        }

        [TestCase("99501", "US")]
        [TestCase("36925", "US")]
        [TestCase("71601", "US")]
        [TestCase("75502", "US")]
        [TestCase("85001", "US")]
        [TestCase("96162", "US")]
        [TestCase("80001", "US")]
        [TestCase("06389", "US")]
        [TestCase("06401", "US")]
        [TestCase("20039", "US")]
        [TestCase("20042", "US")]
        [TestCase("20799", "US")]
        [TestCase("19701", "US")]
        [TestCase("34997", "US")]
        [TestCase("30001", "US")]
        [TestCase("39901", "US")]
        [TestCase("96701", "US")]
        [TestCase("52809", "US")]
        [TestCase("68119", "US")]
        [TestCase("83876", "US")]
        [TestCase("60001", "US")]
        [TestCase("47997", "US")]
        [TestCase("66002", "US")]
        [TestCase("42788", "US")]
        [TestCase("70001", "US")]
        [TestCase("71497", "US")]
        [TestCase("01001", "US")]
        [TestCase("05544", "US")]
        [TestCase("20331", "US")]
        [TestCase("20797", "US")]
        [TestCase("20812", "US")]
        [TestCase("04992", "US")]
        [TestCase("48001", "US")]
        [TestCase("56763", "US")]
        [TestCase("63001", "US")]
        [TestCase("39776", "US")]
        [TestCase("71233", "US")]
        [TestCase("59937", "US")]
        [TestCase("27006", "US")]
        [TestCase("58856", "US")]
        [TestCase("68001", "US")]
        [TestCase("69367", "US")]
        [TestCase("03031", "US")]
        [TestCase("08989", "US")]
        [TestCase("87001", "US")]
        [TestCase("89883", "US")]
        [TestCase("06390", "US")]
        [TestCase("14975", "US")]
        [TestCase("43001", "US")]
        [TestCase("73199", "US")]
        [TestCase("73401", "US")]
        [TestCase("97920", "US")]
        [TestCase("15001", "US")]
        [TestCase("00900", "US")]
        [TestCase("02801", "US")]
        [TestCase("29948", "US")]
        [TestCase("57001", "US")]
        [TestCase("38589", "US")]
        [TestCase("73301", "US")]
        [TestCase("75501", "US")]
        [TestCase("75503", "US")]
        [TestCase("88589", "US")]
        [TestCase("84001", "US")]
        [TestCase("20041", "US")]
        [TestCase("20040", "US")]
        [TestCase("20042", "US")]
        [TestCase("22001", "US")]
        [TestCase("05495", "US")]
        [TestCase("05601", "US")]
        [TestCase("99403", "US")]
        [TestCase("53001", "US")]
        [TestCase("26886", "US")]
        [TestCase("82001", "US")]
        public void WHEN_Passing_Valid_Parameters_United_States_Country_SHOULD_Succeed(string zipCode, string countryCode)
        {
            //Arrange
            var regionProvider = new RegionCodeProvider();

            // Act
            Action action = () => regionProvider.GetRegion(zipCode, countryCode);

            // Assert
            action.ShouldNotThrow();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("99999")]
        public void WHEN_Passing_Invalid_Parameters_Canada_Country_SHOULD_Throw_ArgumentException(string postalCode)
        {
            //Arrange
            var composerContextMock = new Mock<IComposerRequestContext>();

            composerContextMock.Setup(c => c.CountryCode).Returns(CountryCodes.Canada);

            var regionProvider = new RegionCodeProvider();

            // Act
            Action action = () => regionProvider.GetRegion(postalCode, composerContextMock.Object.CountryCode);

            // Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("J5A0G5")]
        [TestCase("1004L")]
        public void WHEN_Passing_Invalid_Parameters_UnitedStates_Country_SHOULD_Throw_ArgumentException(string zipCode)
        {
            //Arrange
            var composerContextMock = new Mock<IComposerRequestContext>();

            composerContextMock.Setup(c => c.CountryCode).Returns(CountryCodes.UnitedStates);

            var regionProvider = new RegionCodeProvider();

            // Act
            Action action = () => regionProvider.GetRegion(zipCode, composerContextMock.Object.CountryCode);

            // Assert
            action.ShouldThrow<ArgumentException>();
        }
    }
}
