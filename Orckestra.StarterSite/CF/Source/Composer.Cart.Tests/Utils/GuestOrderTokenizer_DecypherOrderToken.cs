﻿using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Cart.Utils;

namespace Orckestra.Composer.Cart.Tests.Utils
{
    [TestFixture]
    public class GuestOrderTokenizer_DecypherOrderToken
    {
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("     \t\r\n")]
        [TestCase(null)]
        public void WHEN_token_is_invalid_SHOULD_throw_ArgumentException(string token)
        {
            //Arrange
            
            //Act
            var ex = Assert.Throws<ArgumentException>(() => GuestOrderTokenizer.DecypherOrderToken(token));

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
            var tokenDto = GuestOrderTokenizer.DecypherOrderToken(token);

            //Assert
            tokenDto.Should().BeNull("token is in the wrong format");
        }

        [Test]
        public void WHEN_token_is_valid_SHOULD_return_dto()
        {
            //Arrange
            var param = new OrderToken
            {
                Email = GetRandom.Email(),
                OrderNumber = GetRandom.String(7)
            };

            var token = GuestOrderTokenizer.GenerateOrderToken(param);

            //Act
            var dto = GuestOrderTokenizer.DecypherOrderToken(token);

            //Assert
            dto.Should().NotBeNull();
            dto.Email.Should().NotBeNullOrWhiteSpace();
            dto.Email.Should().Be(param.Email);
            dto.OrderNumber.Should().Be(param.OrderNumber);
        }
    }
}
