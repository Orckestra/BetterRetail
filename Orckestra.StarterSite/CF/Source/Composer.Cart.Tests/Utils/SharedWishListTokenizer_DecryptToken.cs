using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Cart.Utils;

namespace Orckestra.Composer.Cart.Tests.Utils
{
    [TestFixture]
    public class SharedWishListTokenizer_DecryptToken
    {
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("     \t\r\n")]
        [TestCase(null)]
        public void WHEN_token_is_invalid_SHOULD_throw_ArgumentException(string token)
        {
            //Arrange

            //Act
            var ex = Assert.Throws<ArgumentException>(() => SharedWishListTokenizer.DecryptToken(token));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_token_format_is_wrong_SHOULD_return_null()
        {
            //Arrange
            var token = GetRandom.String(17);

            //Act
            var tokenDto = SharedWishListTokenizer.DecryptToken(token);

            //Assert
            tokenDto.Should().BeNull("token is in the wrong format");
        }

        [TestCase("Canada")]
        [TestCase("123456")]
        [TestCase("Canada_123")]
        [TestCase("Canada-123")]
        public void WHEN_token_is_valid_SHOULD_return_dto(string scope)
        {
            //Arrange
            var param = new SharedWishListToken
            {
                CustomerId = Guid.NewGuid(),
                Scope = scope
            };

            var token = SharedWishListTokenizer.GenerateToken(param);

            //Act
            var dto = SharedWishListTokenizer.DecryptToken(token);

            //Assert
            dto.Should().NotBeNull();
            dto.CustomerId.Should().Be(param.CustomerId);
            dto.Scope.Should().Be(param.Scope);
        }
    }
}
