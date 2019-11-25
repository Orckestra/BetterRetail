using System;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class RequestUrilsGetBaseUrl
    {
        [TestCase("http://foo.com/en-CA/p-product-a-is-nice/123456", "http://foo.com/")]
        [TestCase("https://foos.com/fr-CA/p-lacoste-t-shirt-zipblack/858542", "https://foos.com/")]
        public void WHEN_baseUrl_return_expected_result(string baseUrl, string expectedUrl)
        {
            //Arrange
            var uri = new Uri(baseUrl);

            //Act
            var url = RequestUtils.GetBaseUrl(uri).ToString();

            //Assert
            url.Should().BeEquivalentTo(expectedUrl);
        }

        [TestCase("http://foo.com:8081/en-CA/p-product-a-is-nice/123456", "http://foo.com/")]
        [TestCase("https://foos.com:4431/fr-CA/p-lacoste-t-shirt-zipblack/858542", "https://foos.com/")]
        public void WHEN_baseUrl_has_port_SHOULD_return_expected_result_without_port(string baseUrl, string expectedUrl)
        {
            //Arrange
            var uri = new Uri(baseUrl);

            //Act
            var url = RequestUtils.GetBaseUrl(uri).ToString();

            //Assert
            url.Should().BeEquivalentTo(expectedUrl);
        }
    }
}
