using System;
using System.Web;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Services.Cookie
{
    [TestFixture]
    public class ComposerCookieAccesserWrite : ComposerCookieAccesserTestsBase
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            DependencyResolverSetup();

            _container = new AutoMocker();
            //
            _container.Use(HttpRequestFactory.CreateForReadingCookies());
            _container.Use(HttpResponseFactory.CreateForWritingCookies());
            
        }

        [TearDown]
        public void TearDown()
        {
            DependencyResolverTearDown();
        }

        [Test]
        [TestCase("6e931d30f7754b70bffd3820a565e4d4", true)]
        [TestCase("6e931d30f7754b70bffd3820a565e4d4", true)]
        [TestCase(null,                               true)]
        [TestCase("6e931d30f7754b70bffd3820a565e4d4", null)]
        [TestCase(null,                               null)]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed(string customerIdGuid, bool? isGuest)
        {
            // Arrange
            ICookieAccessor<ComposerCookieDto> accessor = _container.CreateInstance<ComposerCookieAccessor>();
            Guid? customerId = customerIdGuid != null ? new Guid(customerIdGuid) : (Guid?)null;
        
            // Act
            ComposerCookieDto dto = accessor.Read();
            dto.EncryptedCustomerId = new EncryptionUtility().Encrypt(customerId.ToString());
            dto.IsGuest    = isGuest;
            accessor.Write(dto);
        
            // Assert
            _container.Get<HttpResponseBase>().Cookies.Count.Should().BeGreaterThan(0);
        }
    }
}
