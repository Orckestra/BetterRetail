using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using Orckestra.Composer.Cart.ViewModels;
using System.Linq.Expressions;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class CartServiceUpdateCartAsync
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
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();

            // Act
            var result = await service.UpdateCartAsync(new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            });

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_Dependencies_Return_NullValues_SHOULD_Succeed()
        {
            var service = _container.CreateInstance<CartService>();
            _container.Use(CartRepositoryFactory.CreateWithNullValues());

            // Act
            var result = await service.UpdateCartAsync(new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            });

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = null,
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCartAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CultureInfo");
        }

        [Test]
        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCartAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("CustomerId");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = cartName,
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.UpdateCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase(" \t\r\n")]
        public void WHEN_BaseUrl_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string baseUrl)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = baseUrl,
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.UpdateCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)));
        }

        [Test]
        public void WHEN_Shipment_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = null,
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCartAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("Shipments");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(" \t\r\n")]
        public void WHEN_BillingCurrency_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string billingCurrency)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = billingCurrency,
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.UpdateCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.BillingCurrency)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartType_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartType)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = cartType,
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.UpdateCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartType)));
        }

        [Test]
        public void WHEN_Coupons_Is_Null_SHOULD_Throw_ArgumentException()
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.Empty,
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = null,
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = GetRandom.String(32)
            };

            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCartAsync(param));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("Coupons");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = scope,
                Status = GetRandom.String(32)
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.UpdateCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(" \t\r\n")]
        public void WHEN_Status_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string status)
        {
            var service = _container.CreateInstance<CartService>();
            var param = new UpdateCartViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
                BaseUrl = GetRandom.String(32),
                Shipments = new List<Shipment>(),
                BillingCurrency = GetRandom.String(32),
                CartType = GetRandom.String(32),
                Coupons = new List<Coupon>(),
                Customer = new CustomerSummary(),
                OrderLocation = new OrderLocationSummary(),
                Scope = GetRandom.String(32),
                Status = status
            };

            // Act
            Expression<Func<Task<CartViewModel>>> expression = () => service.UpdateCartAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Status)));
        }
    }
}
