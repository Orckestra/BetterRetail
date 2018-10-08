using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class UrlFormatterFormatProductName
    {
        [TestCase("red carp pallet", "red-carppallet")]
        [TestCase("test product name", "test-product-name")]
        public void WHEN_productName_contains_word_ending_with_p_SHOULD_skip_dash(string productName,
            string expectedResult)
        {
            //Arrange

            //Act
            var formatted = UrlFormatter.FormatProductName(productName);

            //Assert
            formatted.Should().Be(expectedResult);
        }
    }
}
