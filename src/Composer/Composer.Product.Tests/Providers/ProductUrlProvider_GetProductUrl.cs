using System;
using System.Globalization;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Providers;

namespace Orckestra.Composer.Product.Tests.Providers
{
    [TestFixture]
    public class ProductUrlProviderGetProductUrl
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [TestCase(null, null, null)]
        [TestCase(null, null, null)]
        [TestCase("", null, null)]
        [TestCase(null, "", null)]
        [TestCase(null, null, "")]
        [TestCase(null, null, null)]
        [TestCase("PRD_ID1", "PRD_NAME", null)]
        public void WHEN_invalid_parameter_SHOULD_throw_ArgumentException(string productId, string productName, string culture)
        {
            //Arrange
            var sut = _container.CreateInstance<ProductUrlProvider>();
            var cultureInfo = string.IsNullOrWhiteSpace(culture) ? null : CultureInfo.GetCultureInfo(culture);

            var p = new GetProductUrlParam()
            {                
                CultureInfo = cultureInfo,
                ProductId = productId,
                ProductName = productName
            };

            //Act
            var action = new Action(() => sut.GetProductUrl(p));

            //Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestCase("http://foo.com", "en-CA", "product A is nice", "123456", "http://foo.com/en-CA/p-product-a-is-nice/123456")]
        [TestCase("https://foos.com", "fr-CA", "LACOSTE T-Shirt Zip Black", "858542", "https://foos.com/fr-CA/p-lacoste-t-shirt-zipblack/858542")]
        public void WHEN_parameters_are_valid_SHOULD_return_expected_result(string baseUrl, string culture,
            string productName, string productId, string expectedUrl)
        {
            //Arrange
            var sut = _container.CreateInstance<ProductUrlProvider>();
            var cultureInfo = string.IsNullOrWhiteSpace(culture) ? null : CultureInfo.GetCultureInfo(culture);

            var p = new GetProductUrlParam()
            {                
                CultureInfo = cultureInfo,
                ProductId = productId,
                ProductName = productName
            };

            //Act
            var relativeUrl = sut.GetProductUrl(p);
            var url = string.Concat(baseUrl, relativeUrl);

            //Assert
            url.Should().BeEquivalentTo(expectedUrl);
        }
    }
}
