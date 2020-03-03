using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CartServiceUpdateLineItemAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();

            _container.Use(ViewModelMapperFactory.Create());
            _container.Use(CartRepositoryFactory.Create());
            _container.Use(CartViewModelFactoryMock.Create());
            _container.Use(CountryServiceMock.Create());
            _container.Use(LocalizationProviderFactory.Create());
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var result = service.UpdateLineItemAsync(new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
                Quantity = 1000,
                BaseUrl = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();
            _container.Use(CartRepositoryFactory.CreateWithNullValues());

            // Act
            var result = service.UpdateLineItemAsync(new UpdateLineItemParam
            {
                ScopeId = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                LineItemId = GetRandom.Guid(),
                GiftMessage = GetRandom.String(32),
                GiftWrap = GetRandom.Boolean(),
                Quantity = 1000,
                BaseUrl = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateLineItemAsync(new UpdateLineItemParam
                {
                    ScopeId = scope,
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = GetRandom.Guid(),
                    CartName = GetRandom.String(32),
                    LineItemId = GetRandom.Guid(),
                    GiftMessage = GetRandom.String(32),
                    GiftWrap = GetRandom.Boolean(),
                    Quantity = GetRandom.PositiveInt(),
                    BaseUrl = GetRandom.String(32)
                })
            );

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.Scope");
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateLineItemAsync(new UpdateLineItemParam
                {
                    ScopeId = GetRandom.String(32),
                    CultureInfo = null,
                    CustomerId = GetRandom.Guid(),
                    CartName = GetRandom.String(32),
                    LineItemId = GetRandom.Guid(),
                    GiftMessage = GetRandom.String(32),
                    GiftWrap = GetRandom.Boolean(),
                    Quantity = GetRandom.PositiveInt(),
                    BaseUrl = GetRandom.String(32)
                })
            );

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.CultureInfo");
        }

        [Test]
        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateLineItemAsync(new UpdateLineItemParam
                {
                    ScopeId = GetRandom.String(32),
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = Guid.Empty,
                    CartName = GetRandom.String(32),
                    LineItemId = GetRandom.Guid(),
                    GiftMessage = GetRandom.String(32),
                    GiftWrap = GetRandom.Boolean(),
                    Quantity = GetRandom.PositiveInt(),
                    BaseUrl = GetRandom.String(32)
                })
            );

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.CustomerId");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateLineItemAsync(new UpdateLineItemParam
                {
                    ScopeId = GetRandom.String(32),
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = GetRandom.Guid(),
                    CartName = cartName,
                    LineItemId = GetRandom.Guid(),
                    GiftMessage = GetRandom.String(32),
                    GiftWrap = GetRandom.Boolean(),
                    Quantity = GetRandom.PositiveInt(),
                    BaseUrl = GetRandom.String(32)
                })
            );

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.CartName");
        }

        [Test]
        public void WHEN_LineItemId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateLineItemAsync(new UpdateLineItemParam
                {
                    ScopeId = GetRandom.String(32),
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = GetRandom.Guid(),
                    CartName = GetRandom.String(32),
                    LineItemId = Guid.Empty,
                    GiftMessage = GetRandom.String(32),
                    GiftWrap = GetRandom.Boolean(),
                    Quantity = GetRandom.PositiveInt(),
                    BaseUrl = GetRandom.String(32)
                })
            );

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.LineItemId");
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void WHEN_Quantity_Is_Not_Positive_SHOULD_Throw_ArgumentException(int quantity)
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() =>
                service.UpdateLineItemAsync(new UpdateLineItemParam
                {
                    ScopeId = GetRandom.String(32),
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = GetRandom.Guid(),
                    CartName = GetRandom.String(32),
                    LineItemId = GetRandom.Guid(),
                    GiftMessage = GetRandom.String(32),
                    GiftWrap = GetRandom.Boolean(),
                    Quantity = quantity,
                    BaseUrl = GetRandom.String(32)
                })
            );

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param.Quantity");
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateLineItemAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}
