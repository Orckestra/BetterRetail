using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Tests.Mock;

namespace Orckestra.Composer.Tests.Services.Cookie
{
    [TestFixture]
    public class ComposerCookieAccesserRead
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            _container.Use(HttpRequestFactory.CreateForReadingCookies());
            _container.Use(HttpResponseFactory.CreateDefault());
        }

        [TearDown]
        public void TearDown()
        {
            //_container.VerifyAll();
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            ICookieAccessor<ComposerCookieDto> accessor = _container.CreateInstance<ComposerCookieAccessor>();
        
            // Act
            ComposerCookieDto dto = accessor.Read();
        
            // Assert
            dto.Should().NotBeNull();
        }
    }
}
