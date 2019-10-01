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
    public class ComposerCookieAccesserWrite
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            //
            _container.Use(HttpRequestFactory.CreateForReadingCookies());
            _container.Use(HttpResponseFactory.CreateForWritingCookies());
            
        }

        [TearDown]
        public void TearDown()
        {
            //_container.VerifyAll();
        }

        [Test]
        [TestCase("Quebec", "6e931d30f7754b70bffd3820a565e4d4", true, "All entries")]
        [TestCase(null,     "6e931d30f7754b70bffd3820a565e4d4", true, "It is ok to omit the Scope")]
        [TestCase("Quebec", null,                               true, "It is ok to omit the Guid")]
        [TestCase("Quebec", "6e931d30f7754b70bffd3820a565e4d4", null, "It is ok to omit the isGuest")]
        [TestCase(null,     null,                               null, "It's actually ok to omit everything")]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed(string scope, string customerIdGuid, bool? isGuest, string comment)
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
            HttpCookie cookie = _container.Get<HttpResponseBase>().Cookies.Get(".Composer");
            cookie.Value.Should().NotBeNullOrWhiteSpace("We just stored something in the cookie");
        }
    }
}
