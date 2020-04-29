using System;
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
    public class CartRepositoryUpdateLineItemAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();

            // Act
            var result = repository.UpdateLineItemAsync(new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                Quantity = 1000,
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.CreateWithNullValues());
            var repository = _container.CreateInstance<CartRepository>();

            // Act
            var result = repository.UpdateLineItemAsync(new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                Quantity = 1000,
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new UpdateLineItemParam
            {
                ScopeId = null,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                Quantity = GetRandom.PositiveInt(),
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.UpdateLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)));
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                Quantity = GetRandom.PositiveInt(),
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.UpdateLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [Test]
        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                Quantity = GetRandom.PositiveInt(),
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.UpdateLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.CustomerId)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = null,
                LineItemId = GetRandom.Guid(),
                Quantity = GetRandom.PositiveInt(),
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.UpdateLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_LineItemId_Is_Empty_SHOULD_Throw_ArgumentException(string productId)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = Guid.Empty,
                Quantity = GetRandom.PositiveInt(),
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.UpdateLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.LineItemId)));
        }
      
        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void WHEN_Quantity_Is_Not_Positive_SHOULD_Throw_ArgumentOutOfRangeException(int quantity)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                Quantity = quantity,
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.UpdateLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfZeroNegative(nameof(param.Quantity)));
        }


        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateLineItemAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}
