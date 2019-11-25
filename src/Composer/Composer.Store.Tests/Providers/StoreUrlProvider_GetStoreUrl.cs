using System;
using System.Globalization;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;

namespace Composer.Store.Tests.Providers
{
    [TestFixture]
    public class StoreUrlProvider_GetStoreUrl
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [TestCase(null, null, null, null)]
        [TestCase("", null, null, null)]
        [TestCase(null, "", null, null)]
        [TestCase(null, null, "", null)]
        [TestCase(null, null, null, "")]
        [TestCase("THIS IS NOT A URL", null, null, null)]
        [TestCase("THIS IS NOT A URL", "STR_NUM1", "STR_NAME", null)]
        public void WHEN_invalid_parameter_SHOULD_throw_ArgumentException(string baseUrl, string storeNumber,
            string storeName, string culture)
        {
            //Arrange
            var sut = _container.CreateInstance<StoreUrlProvider>();
            var cultureInfo = string.IsNullOrWhiteSpace(culture) ? null : CultureInfo.GetCultureInfo(culture);

            var sp = new GetStoreUrlParam
            {
                BaseUrl = baseUrl,
                CultureInfo = cultureInfo,
                StoreNumber = storeNumber,
                StoreName = storeName
            };

            //Act
            var action = new Action(() => sut.GetStoreUrl(sp));

            //Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestCase("http://foo.com", "en-CA", "BRC-482", "BRC Avalon Mall",
            "http://foo.com/en-CA//BRC-482-s-brc-avalon-mall")]
        [TestCase("https://foos.com", "fr-CA", "BRC101", "1st Street",
            "https://foos.com/fr-CA//BRC101-s-1st-street")]
        public void WHEN_parameters_are_valid_SHOULD_return_expected_result(string baseUrl, string culture,
            string storeNumber, string storeName, string expectedUrl)
        {
            //Arrange
            var sut = _container.CreateInstance<StoreUrlProvider>();
            var cultureInfo = string.IsNullOrWhiteSpace(culture) ? null : CultureInfo.GetCultureInfo(culture);

            var sp = new GetStoreUrlParam
            {
                BaseUrl = baseUrl,
                CultureInfo = cultureInfo,
                StoreNumber = storeNumber,
                StoreName = storeName
            };

            //Act
            var relativeUrl = sut.GetStoreUrl(sp);
            var url = string.Concat(baseUrl, relativeUrl);

            //Assert
            url.Should().BeEquivalentTo(expectedUrl);
        }
    }
}
