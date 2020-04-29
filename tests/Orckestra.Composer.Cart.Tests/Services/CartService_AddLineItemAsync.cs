using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.ForTests;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CartServiceAddLineItemAsync
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
            _container.Use<IFixCartService>(new FakeFixCartService());

            var localizationProviderMock = new Mock<ILocalizationProvider>();
            localizationProviderMock
                .Setup(c => c.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                .Returns("{0}");
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var result = service.AddLineItemAsync(new AddLineItemParam
            {
                Scope       = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId  = GetRandom.Guid(),
                CartName    = GetRandom.String(32),
                ProductId   = GetRandom.String(32),
                VariantId   = GetRandom.String(32),
                Quantity    = 1000,
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
            var result = service.AddLineItemAsync(new AddLineItemParam
            {
                Scope       = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId  = GetRandom.Guid(),
                CartName    = GetRandom.String(32),
                ProductId   = GetRandom.String(32),
                VariantId   = GetRandom.String(32),
                Quantity    = 1000,
                BaseUrl =  GetRandom.String(32)
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
            var param = new AddLineItemParam
            {
                Scope = scope,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.PositiveInt(),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.PositiveInt(),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [Test]
        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.PositiveInt(),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.AddLineItemAsync(param);
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
            var service = _container.CreateInstance<CartService>();
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = cartName,
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.PositiveInt(),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.AddLineItemAsync(param);
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
            var service = _container.CreateInstance<CartService>();
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = productId,
                VariantId = GetRandom.String(32),
                Quantity = GetRandom.PositiveInt(),
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ProductId)));
        }


        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_VariantID_Is_NullOrWhitespace_SHOULD_Succeed(string variantId)
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var result = service.AddLineItemAsync(new AddLineItemParam
            {
                Scope       = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId  = GetRandom.Guid(),
                CartName    = GetRandom.String(32),
                ProductId   = GetRandom.String(32),
                VariantId   = variantId,
                Quantity    = 1000,
                BaseUrl = GetRandom.String(32)
            }).Result;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void WHEN_Quantity_Is_Not_Positive_SHOULD_Throw_ArgumentOutOfRangeException(int quantity)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new AddLineItemParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Quantity = quantity,
                BaseUrl = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.AddLineItemAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddLineItemAsync(param));

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfZeroNegative(nameof(param.Quantity)));
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.AddLineItemAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }
    }
}
