using System.Globalization;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.Localization
{
    [TestFixture]
    public class LocalizationExtensionsFormatPrice : BaseTestForStaticSut
    {
        [TestCase(-1, "($1.00)", "{0:C2}")]
        [TestCase(-1.30, "($1.30)", "{0:C}")]
        [TestCase(-1.35, "($1.35)", "{0:C}")]
        [TestCase(-1.30323, "($1.30)", "{0:C}")]
        [TestCase(-1.30623, "($1.31)", "{0:C}")]
        [TestCase(2, "$2.00", "{0:C}")]
        [TestCase(2.30, "$2.30", "{0:C}")]
        [TestCase(2.35, "$2.35", "{0:C}")]
        [TestCase(2.3514322, "$2.35", "{0:C}")]
        [TestCase(2.3554322, "$2.36", "{0:C}")]
        [TestCase(-1, "($1.00)CAN", "{0:C2}CAN")]
        [TestCase(-1.30, "($1.30)CAN", "{0:C}CAN")]
        [TestCase(-1.35, "($1.35)CAN", "{0:C}CAN")]
        [TestCase(-1.30323, "($1.30)CAN", "{0:C}CAN")]
        [TestCase(-1.30623, "($1.31)CAN", "{0:C}CAN")]
        [TestCase(2, "$2.00CAN", "{0:C}CAN")]
        [TestCase(2.30, "$2.30CAN", "{0:C}CAN")]
        [TestCase(2.35, "$2.35CAN", "{0:C}CAN")]
        [TestCase(2.3514322, "$2.35CAN", "{0:C}CAN")]
        [TestCase(2.3554322, "$2.36CAN", "{0:C}CAN")]
        public void WHEN_passing_valid_values_decimal_SHOULD_return_formated_text_currency(decimal price, string formatExpected, string format)
        {
            //Arrange
            var localizationProvider = new Mock<ILocalizationProvider>();
            localizationProvider.Setup(c => c.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                                .Returns(format);

            //Act
            var formatedPrice = localizationProvider.Object.FormatPrice(price, CultureInfo.CreateSpecificCulture("en-US"));

            //Assert
            formatedPrice.Should().Be(formatedPrice, formatExpected);
        }

        [Test]
        public void WHEN_passing_max_and_min_value_SHOULD_return_formated_max_and_min_values()
        {
            //Arrange
            var localizationProvider = new Mock<ILocalizationProvider>();
            localizationProvider.Setup(c => c.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                                .Returns("{0:C}");
            var enUsCulture = CultureInfo.CreateSpecificCulture("en-US");

            //Act
            var formatedPriceMax = localizationProvider.Object.FormatPrice(decimal.MaxValue, enUsCulture);
            var formatedPriceMin = localizationProvider.Object.FormatPrice(decimal.MinValue, enUsCulture);

            //Assert
            formatedPriceMax.Should().Be(string.Format(enUsCulture, "{0:C}", decimal.MaxValue));
            formatedPriceMin.Should().Be(string.Format(enUsCulture, "{0:C}", decimal.MinValue));
        }
    }
}
