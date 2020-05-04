using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class CartRepository_AddLineItemAsync
    {
        private AutoMocker _container;
        private readonly ProcessedCart _dummyCart = new ProcessedCart
        {
            Shipments = new List<Shipment>
            {
                new Shipment
                {
                    FulfillmentLocationId = GetRandom.Guid()
                }
            }
        };

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            //var inventoryProviderMock = _container.GetMock<IInventoryLocationProvider>();
            //inventoryProviderMock.Setup(
            //    invProv => invProv.GetFulfillmentLocationAsync(It.IsNotNull<GetFulfillmentLocationParam>()))
            //    .ReturnsAsync(new FulfillmentLocation
            //    {
            //        Id = GetRandom.Guid()
            //    });
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.CreateMockWithValue(_dummyCart));
            var repository = _container.CreateInstance<CartRepository>();

            // Act
            var result = repository.AddLineItemAsync(new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.Int(1, 10000)
            }).Result;

            // Assert
            result.Should().NotBeNull("Missing CustomerID or Cart are expected to be created automatically");
        }

        [Test]
        public void WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            //Arrange
            _container.Use(OvertureClientFactory.CreateWithNullValues());

            var repository = _container.CreateInstance<CartRepository>();

            // Act
            var result = repository.AddLineItemAsync(new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.Int(1, 1000)
            }).Result;

            // Assert
            result.Should().NotBeNull("Missing CustomerID or Cart are expected to be created automatically");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new AddLineItemParam
            {
                Scope = scope,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.Int(1, 10)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.Int(1, 100)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
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
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.Int(1, 1000)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
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
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = cartName,
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.Int(0, 1000)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_ProductId_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string productId)
        {
            // Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = productId,
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.Int(0, 1000)
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ProductId)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_VariantId_Is_NullOrWhitespace_SHOULD_Succeed(string variantId)
        {
            //Arrange
            _container.Use(OvertureClientFactory.Create());
            var repository = _container.CreateInstance<CartRepository>();

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
            result.Should().NotBeNull("Missing CustomerID or Cart are expected to be created automatically");
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
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = quantity
            };

            // Act
            Expression<Func<Task<ProcessedCart>>> expression = () => repository.AddLineItemAsync(param);
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
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddLineItemAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}
