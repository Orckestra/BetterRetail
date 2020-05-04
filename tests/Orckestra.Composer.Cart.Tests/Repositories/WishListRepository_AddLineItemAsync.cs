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
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Orders;

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

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new AddLineItemParam
            {
                Scope = scope,
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = "WishList",
                ProductId = "ProductId",
                VariantId = "VariantId",
                Quantity = 1
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new AddLineItemParam
            {
                Scope = "Canada",
                CultureInfo = null,
                CustomerId = Guid.NewGuid(),
                CartName = "WishList",
                ProductId = "ProductId",
                VariantId = "VariantId",
                Quantity = 1
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_ProductId_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string productId)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new AddLineItemParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = "WishList",
                ProductId = productId,
                VariantId = "VariantId",
                Quantity = 1
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ProductId)));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void WHEN_Quantity_Is_ZeroNegative_SHOULD_Throw_ArgumentException(int quantity)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new AddLineItemParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = "WishList",
                ProductId = "ProductId",
                VariantId = "VariantId",
                Quantity = quantity
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfZeroNegative(nameof(param.Quantity)));
        }

        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<WishListRepository>();
            var param = new AddLineItemParam
            {
                Scope = "Canada",
                CultureInfo = null,
                CustomerId = Guid.Empty,
                CartName = "WishList",
                ProductId = "ProductId",
                VariantId = "VariantId",
                Quantity = 1
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.CustomerId)));
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
                Quantity = 1000
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
