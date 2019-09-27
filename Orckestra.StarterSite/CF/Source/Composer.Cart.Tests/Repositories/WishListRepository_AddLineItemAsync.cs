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
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class WishListRepository_AddLineItemAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [TestCase(null, "en-CA", "WishList", "randomCustomerId", "productId0001", 1, "param.Scope")]
        [TestCase("", "en-CA", "WishList", "randomCustomerId", "productId0001", 1, "param.Scope")]
        [TestCase("Canada", null, "WishList", "randomCustomerId", "productId0001", 1, "param.CultureInfo")]
        [TestCase("Canada", "en-CA", "WishList", "randomCustomerId", null, 1, "param.ProductId")]
        [TestCase("Canada", "en-CA", "WishList", "randomCustomerId", "", 1, "param.ProductId")]
        [TestCase("Canada", "en-CA", "WishList", "randomCustomerId", "productId0001", 0, "param.Quantity")]
        [TestCase("Canada", "en-CA", "WishList", null, "productId0001", 1, "param.CustomerId")]
        public void WHEN_Param_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope, string cultureName,
            string cartName, string customerId, string productId, int quantity, string paramName)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new AddLineItemParam
            {
                Scope = scope,
                CultureInfo =
                        string.IsNullOrWhiteSpace(cultureName) ? null : CultureInfo.GetCultureInfo(cultureName),
                CustomerId = string.IsNullOrWhiteSpace(customerId) ? Guid.Empty : GetRandom.Guid(),
                CartName = cartName,
                ProductId = productId,
                VariantId = GetRandom.String(10),
                Quantity = quantity
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => repository.AddLineItemAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain(paramName);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_VariantId_Is_NullOrWhitespace_SHOULD_Succeed(string variantId)
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();

            // Act
            var result = repository.AddLineItemAsync(new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = variantId,
                Quantity = GetRandom.Int(0, 1000)
            }).Result;

            // Assert
            result.Should().NotBeNull("Missing CustomerID or WishList are expected to be created automatically");
        }


        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddLineItemAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}
