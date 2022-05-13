using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Utils;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

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
            Expression<Func<Uri>> expression = () => UrlFormatter.GetUriFromString(url);
            var exception = Assert.Throws<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
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
