using System;
using System.Web;
using Autofac;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Kernel;
using Orckestra.Composer.Services.Cookie;

namespace Orckestra.Composer.Tests.Services.Cookie
{
    [TestFixture]
    public class ComposerCookieAccesserCtor : ComposerCookieAccesserTestsBase
    {

        [SetUp]
        public void SetUp()
        {
            DependencyResolverSetup();
        }

        [TearDown]
        public void TearDown()
        {
            DependencyResolverTearDown();
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            Mock<HttpRequestBase>  httpRequest  = new Mock<HttpRequestBase>();
            Mock<HttpResponseBase> httpResponse = new Mock<HttpResponseBase>();

            // Act
            Action action = () => new ComposerCookieAccessor(httpRequest.Object, httpResponse.Object);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_Passing_Null_HttpRequest_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            Mock<HttpResponseBase> httpResponse = new Mock<HttpResponseBase>();

            // Act
            Action action = () => new ComposerCookieAccessor(null, httpResponse.Object);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_HttpResponse_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            Mock<HttpRequestBase>  httpRequest  = new Mock<HttpRequestBase>();

            // Act
            Action action = () => new ComposerCookieAccessor(httpRequest.Object, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
