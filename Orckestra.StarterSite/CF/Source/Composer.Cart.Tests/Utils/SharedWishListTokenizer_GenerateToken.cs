using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Cart.Utils;

namespace Orckestra.Composer.Cart.Tests.Utils
{
    [TestFixture]
    public class SharedWishListTokenizer_GenerateToken
    {
        [Test]
        public void WHEN_token_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            SharedWishListToken param = null;

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => SharedWishListTokenizer.GenerateToken(param));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase("")]
        [TestCase("     ")]
        [TestCase("     \t\r\n")]
        [TestCase(null)]
        public void WHEN_Scope_is_invalid_SHOULD_throw_ArgumentNullException(string scope)
        {
            //Arrange
            SharedWishListToken param = new SharedWishListToken
            {
                CustomerId = Guid.NewGuid(),
                Scope = scope
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(() => SharedWishListTokenizer.GenerateToken(param));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
            ex.Message.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase]
        public void WHEN_CustomerId_is_invalid_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            SharedWishListToken param = new SharedWishListToken
            {
                CustomerId = Guid.Empty,
                Scope = GetRandom.String(6)
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(() => SharedWishListTokenizer.GenerateToken(param));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
            ex.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_token_ok_SHOULD_return_encrypted_token()
        {
            //Arrange
            SharedWishListToken param = new SharedWishListToken
            {
                CustomerId = Guid.NewGuid(),
                Scope = GetRandom.String(6)
            };

            //Act
            var token = SharedWishListTokenizer.GenerateToken(param);

            //Assert
            token.Should().NotBeNullOrWhiteSpace();
            token.Should().NotContainEquivalentOf(param.CustomerId.ToString());
            token.Should().NotContainEquivalentOf(param.Scope);
        }
    }
}
