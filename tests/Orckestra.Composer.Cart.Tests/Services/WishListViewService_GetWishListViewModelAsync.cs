using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Providers.Dam;
using Orckestra.ForTests;
using Orckestra.Overture.ServiceModel.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Tests.Services
{
    public class WishListViewService_GetWishListViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            //Arrange
            _container = new AutoMocker();
            _container.Use(CreateWishListUrlProvider());
            _container.Use(CreateLineItemService());
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string scope)
        {
            // Arrange
            var service = _container.CreateInstance<WishListViewService>();
            var param = new GetCartParam
            {
                Scope = scope,
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = "WishList"
            };

            // Act
            Expression<Func<Task<WishListViewModel>>> expression = () => service.GetWishListViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.Scope)));
        }

        public void WHEN_CultureInfo_Is_Null_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            var service = _container.CreateInstance<WishListViewService>();
            var param = new GetCartParam
            {
                Scope = "Canada",
                CultureInfo = null,
                CustomerId = Guid.NewGuid(),
                CartName = "WishList"
            };

            // Act
            Expression<Func<Task<WishListViewModel>>> expression = () => service.GetWishListViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNull(nameof(param.CultureInfo)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_CartName_Is_NullOrWhitespace_SHOULD_Throw_ArgumentException(string cartName)
        {
            // Arrange
            var service = _container.CreateInstance<WishListViewService>();
            var param = new GetCartParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.NewGuid(),
                CartName = cartName
            };

            // Act
            Expression<Func<Task<WishListViewModel>>> expression = () => service.GetWishListViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CartName)));
        }

        public void WHEN_CustomerId_Is_Empty_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            var service = _container.CreateInstance<WishListViewService>();
            var param = new GetCartParam
            {
                Scope = "Canada",
                CultureInfo = CultureInfo.GetCultureInfo("en-CA"),
                CustomerId = Guid.Empty,
                CartName = "WishList"
            };

            // Act
            Expression<Func<Task<WishListViewModel>>> expression = () => service.GetWishListViewModelAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            //Assert
            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfEmpty(nameof(param.CustomerId)));
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            var service = _container.CreateInstance<WishListViewService>();

            // Act
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.GetWishListViewModelAsync(null));

            //Assert
            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("param");
        }

        [Test]
        public void WHEN_WishList_with_Items_SHOULD_setup_TotalQuantity()
        {
            //Arrange
            var wishList = CreateWishListWithLineItems();
            MockFixCartService();
            _container.Use(WishListRepositoryFactory.CreateWithValues(wishList));
            var service = _container.CreateInstance<WishListViewService>();

            //Act
            var vm = service.GetWishListViewModelAsync(new GetCartParam
            {
                Scope = "Canada",
                BaseUrl = GetRandom.String(128),
                CartName = "WishList",
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid()
            }).Result;

            //Assert
            vm.TotalQuantity.Should().Be(2);
        }

        [Test]
        public void WHEN_WishList_with_no_Items_SHOULD_ShareUrl_Be_Empty()
        {
            //Arrange
            _container.Use(WishListRepositoryFactory.CreateWithNullValues());
            MockFixCartService();
            var service = _container.CreateInstance<WishListViewService>();

            //Act
            var vm = service.GetWishListViewModelAsync(new GetCartParam
            {
                Scope = "Canada",
                BaseUrl = GetRandom.String(128),
                CartName = "WishList",
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid()
            }).Result;

            //Assert
            vm.ShareUrl.Should().BeEmpty();
        }

        [Test]
        public void WHEN_WishList_with_Items_SHOULD_setup_ShareUrl()
        {
            //Arrange
            var wishList = CreateWishListWithLineItems();
            MockFixCartService();
            _container.Use(WishListRepositoryFactory.CreateWithValues(wishList));
            var service = _container.CreateInstance<WishListViewService>();

            //Act
            var vm = service.GetWishListViewModelAsync(new GetCartParam
            {
                Scope = "Canada",
                BaseUrl = GetRandom.String(128),
                CartName = "WishList",
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid()
            }).Result;

            //Assert
            vm.ShareUrl.Should().NotBeEmpty();
        }

        [Test]
        public void WHEN_GetWishList_SHOULD_setup_SignInUrl()
        {
            //Arrange
            var wishList = CreateWishListWithLineItems();
            MockFixCartService();
            _container.Use(WishListRepositoryFactory.CreateWithValues(wishList));
            var service = _container.CreateInstance<WishListViewService>();

            //Act
            var vm = service.GetWishListViewModelAsync(new GetCartParam
            {
                Scope = "Canada",
                BaseUrl = GetRandom.String(128),
                CartName = "WishList",
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid()
            }).Result;

            //Assert
            vm.SignInUrl.Should().NotBeEmpty();
       }

        private void MockFixCartService()
        {
            var fixCartService = _container.GetMock<IFixCartService>();
            fixCartService
                .Setup(s => s.SetFulfillmentLocationIfRequired(It.IsAny<FixCartParam>()))
                .Returns<FixCartParam>(x => Task.FromResult(x.Cart));
        }

        private Mock<IWishListUrlProvider> CreateWishListUrlProvider()
        {
            Mock<IWishListUrlProvider> urlProvider = new Mock<IWishListUrlProvider>();

            urlProvider.Setup(p => p.GetShareUrl(It.IsAny<GetShareWishListUrlParam>()))
                .Returns(() => GetRandom.String(128))
                .Verifiable();

            urlProvider.Setup(p => p.GetSignInUrl(It.IsAny<GetWishListUrlParam>()))
                .Returns(() => GetRandom.String(128))
                .Verifiable();

            return urlProvider;
        }

        private Mock<ILineItemService> CreateLineItemService()
        {
            Mock<ILineItemService> service = new Mock<ILineItemService>();
            
            return service;
        }

        private ProcessedCart CreateWishListWithLineItems()
        {
            ProcessedCart cart = new ProcessedCart();
            cart.Id = Guid.NewGuid();
            cart.CustomerId = Guid.NewGuid();
            var shipment = new Shipment();
            shipment.LineItems = new List<LineItem>();
            shipment.LineItems.Add(new LineItem
            {
                ProductId = "0001",
                Id = Guid.NewGuid()
            });
            shipment.LineItems.Add(new LineItem
            {
                ProductId = "0002",
                Id = Guid.NewGuid()
            });
            cart.Shipments = new List<Shipment> { shipment };

            return cart;
        }
   }
}
