using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class UrlFormatter_GetUriFromString
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("         ")]
        [TestCase("  \r\n   \t")]
        public void WHEN_url_invalid_SHOULD_throw_ArgumentException(string url)
        {
            //Arrange

            //Act
            var ex = Assert.Throws<ArgumentException>(() => UrlFormatter.GetUriFromString(url));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
            ex.Message.Should().ContainEquivalentOf("url");
        }

        [TestCase("https://google.ca")]
        [TestCase("http://bing.com")]
        [TestCase("https://beta.mondou.com")]
        public void WHEN_url_absolute_SHOULD_return_uri(string url)
        {
            //Arrange

            //Act
            Uri uri = null;
            Assert.DoesNotThrow(() => uri = UrlFormatter.GetUriFromString(url));

            //Assert
            uri.Should().NotBeNull();
            uri.ToString().Should().StartWithEquivalent(url);
            
        }
        
        [TestCase("/en-CA/test/index")]
        [TestCase("/api/test")]
        [TestCase("/fr-CA/")]
        public void WHEN_url_relative_SHOULD_return_uri(string url)
        {
            //Arrange

            //Act
            Uri uri = null;
            Assert.DoesNotThrow(() => uri = UrlFormatter.GetUriFromString(url));

            //Assert
            uri.Should().NotBeNull();
            uri.ToString().ShouldBeEquivalentTo(url);
        }
    }
}
