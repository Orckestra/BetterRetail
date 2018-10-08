using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Cart.Utils;

namespace Orckestra.Composer.Cart.Tests.Utils
{
    [TestFixture]
    public class GuestOrderTokenizer_GenerateOrderToken
    {
        [Test]
        public void WHEN_token_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            OrderToken param = null;

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => GuestOrderTokenizer.GenerateOrderToken(param));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        }
        
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("     \t\r\n")]
        [TestCase(null)]
        public void WHEN_email_is_invalid_SHOULD_throw_ArgumentNullException(string email)
        {
            //Arrange
            OrderToken param = new OrderToken
            {
                Email = email,
                OrderNumber = GetRandom.String(6)
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(() => GuestOrderTokenizer.GenerateOrderToken(param));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
            ex.Message.Should().NotBeNullOrWhiteSpace();
        }
        
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("     \t\r\n")]
        [TestCase(null)]
        public void WHEN_orderNumber_is_invalid_SHOULD_throw_ArgumentNullException(string orderNumber)
        {
            //Arrange
            OrderToken param = new OrderToken
            {
                Email = GetRandom.Email(),
                OrderNumber = orderNumber
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(() => GuestOrderTokenizer.GenerateOrderToken(param));

            //Assert
            ex.Should().NotBeNull();
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
            ex.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_token_ok_SHOULD_return_encrypted_token()
        {
            //Arrange
            var param = new OrderToken
            {
                Email = GetRandom.Email(),
                OrderNumber = GetRandom.String(6)
            };

            //Act
            var token = GuestOrderTokenizer.GenerateOrderToken(param);

            //Assert
            token.Should().NotBeNullOrWhiteSpace();
            token.Should().NotContainEquivalentOf(param.Email);
            token.Should().NotContainEquivalentOf(param.OrderNumber);
        }
    }
}
