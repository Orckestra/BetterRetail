using System;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    public class WishListRepository_RemoveLineItemAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();
        }

        [Test]
        public async void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<WishListRepository>();

            // Act
            var result = await cartRepository.RemoveLineItemAsync(new RemoveLineItemParam
            {
                Scope       = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId  = GetRandom.Guid(),
                CartName    = GetRandom.String(32),
                LineItemId = GetRandom.Guid()
            });

            // Assert
            result.Should().NotBeNull();
        }

        [TestCase(null, "en-CA", "WishList", "randomCustomerId", "randomLineItemId", "param.Scope")]
        [TestCase("", "en-CA", "WishList", "randomCustomerId", "randomLineItemId", "param.Scope")]
        [TestCase("Canada", null, "WishList", "randomCustomerId", "randomLineItemId", "param.CultureInfo")]
        [TestCase("Canada", "en-CA", null, "randomCustomerId", "randomLineItemId", "param.CartName")]
        [TestCase("Canada", "en-CA", "", "randomCustomerId", "randomLineItemId", "param.CartName")]
        [TestCase("Canada", "en-CA", "WishList", null, "randomLineItemId", "param.CustomerId")]
        [TestCase("Canada", "en-CA", "WishList", "randomCustomerId", null, "param.LineItemId")]
        public void WHEN_Param_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope, string cultureName, string cartName,
            string customerId, string lineItemId, string paramName)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();

            // Act
            var exception = Assert.Throws<ArgumentException>(async () =>
            {
                await repository.RemoveLineItemAsync(new RemoveLineItemParam
                {
                    Scope = scope,
                    CultureInfo = string.IsNullOrWhiteSpace(cultureName) ? null : CultureInfo.GetCultureInfo(cultureName),
                    CustomerId = string.IsNullOrWhiteSpace(customerId) ? Guid.Empty : GetRandom.Guid(),
                    CartName = cartName,
                    LineItemId = string.IsNullOrWhiteSpace(lineItemId) ? Guid.Empty : GetRandom.Guid()
                });
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain(paramName);
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var cartRepository = _container.CreateInstance<WishListRepository>();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(async () =>
            {
                await cartRepository.RemoveLineItemAsync(null);
            });

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}
