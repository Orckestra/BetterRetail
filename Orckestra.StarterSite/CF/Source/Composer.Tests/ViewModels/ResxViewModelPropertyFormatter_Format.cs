using System;
using System.Globalization;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    // ReSharper disable once InconsistentNaming
    public class ResxViewModelPropertyFormatter_Format : BaseTest
    {
        public CultureInfo EnglishCanadaCulture = new CultureInfo("en-CA");
        public CultureInfo FrenchCanadaCulture = new CultureInfo("fr-CA");
        [Test]
        public void WHEN_Format_Called_with_DateTime_SHOULD_Format_Date()
        {
            // Arrange
            var localizationProvicerMock = Container.GetMock<ILocalizationProvider>();
            localizationProvicerMock.Setup(m => m.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                                    .Returns("{0:MMMM d, yyyy}");

            var formatter = Container.CreateInstance<ResxViewModelPropertyFormatter>();
            var propertyMetadataMock = new Mock<IPropertyMetadata>();


            // Act
            var result = formatter.Format(new DateTime(2015, 04, 19), propertyMetadataMock.Object, EnglishCanadaCulture);

            // Assert
            result.Should().Be("April 19, 2015");

        }

        [Test]
        public void WHEN_ILocalizationProvider_Returns_Null_SHOULD_Call_ToString()
        {
            // Arrange
            var localizationProvicerMock = Container.GetMock<ILocalizationProvider>();
            localizationProvicerMock.Setup(m => m.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                                    .Returns((string)null);

            var formatter = Container.CreateInstance<ResxViewModelPropertyFormatter>();
            var propertyMetadataMock = new Mock<IPropertyMetadata>();
            var value = new DateTime(2015, 04, 19);

            // Act
            var result = formatter.Format(value, propertyMetadataMock.Object, EnglishCanadaCulture);

            // Assert
            result.Should().Be(value.ToString());
        }

        [Test]
        public void WHEN_Model_Value_Is_Null_SHOULD_Return_Null()
        {
            var formatter = Container.CreateInstance<ResxViewModelPropertyFormatter>();
            var propertyMetadataMock = new Mock<IPropertyMetadata>();


            // Act
            var result = formatter.Format(null, propertyMetadataMock.Object, EnglishCanadaCulture);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void WHEN_IPropertyMetaData_Is_Null_SHOULD_Throw_Exception()
        {
            var formatter = Container.CreateInstance<ResxViewModelPropertyFormatter>();

            // Assert
            formatter.Invoking(f => f.Format("value", null, EnglishCanadaCulture)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Culture_Is_Null_SHOULD_Throw_Exception()
        {
            // Arrange
            var formatter = Container.CreateInstance<ResxViewModelPropertyFormatter>();
            var propertyMetadataMock = new Mock<IPropertyMetadata>();

            // Assert
            formatter.Invoking(f => f.Format("value", propertyMetadataMock.Object, null)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Formatting_Currency_SHOULD_Use_Culture()
        {
            // Arrange
            var formatter = Container.CreateInstance<ResxViewModelPropertyFormatter>();
            var propertyMetaDataMock = new Mock<IPropertyMetadata>();
            var localizationProvicerMock = Container.GetMock<ILocalizationProvider>();
            localizationProvicerMock.Setup(m => m.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                                    .Returns("{0:C}");

            // Act
            var frenchResult = formatter.Format(99.99m, propertyMetaDataMock.Object, FrenchCanadaCulture);
            var englishResult = formatter.Format(99.99m, propertyMetaDataMock.Object, EnglishCanadaCulture);

            // Assert
            frenchResult.Should().Be("99,99 $");
            englishResult.Should().Be("$99.99");
        }

    }
}
