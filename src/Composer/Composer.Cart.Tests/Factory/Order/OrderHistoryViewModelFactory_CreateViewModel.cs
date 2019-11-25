using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Factory.Order
{
    public class OrderHistoryViewModelFactoryCreateViewModel
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<ILocalizationProvider>(MockBehavior.Strict));
            _container.Use(MockViewModelMapperFactory.Create(typeof(LightOrderDetailViewModel).Assembly));

            _container.GetMock<ILocalizationProvider>()
            .Setup(r => r.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
            .Returns(GetRandom.String(32));
        }

        [Test]
        public void WHEN_orderResult_is_null_SHOULD_return_null()
        {
            //Arrange
            var factory = _container.CreateInstance<OrderHistoryViewModelFactory>();

            //Act
            var param = new GetOrderHistoryViewModelParam
            {
                CultureInfo = new CultureInfo("en-US"),
                Page = 1,
                OrderStatuses = new Dictionary<string, string>(),
                OrderDetailBaseUrl = GetRandom.String(32)
            };
            var vm = factory.CreateViewModel(param);

            //Assert
            vm.Should().BeNull();
        }

        [Test]
        public void WHEN_orderResult_results_is_null_SHOULD_return_null()
        {
            //Arrange
            var factory = _container.CreateInstance<OrderHistoryViewModelFactory>();

            //Act
            var param = new GetOrderHistoryViewModelParam
            {
                CultureInfo = new CultureInfo("en-US"),
                Page = 1,
                OrderResult = new OrderQueryResult
                {
                    Results = null
                },
                OrderStatuses = new Dictionary<string, string>(),
                OrderDetailBaseUrl = GetRandom.String(32)
            };
            var vm = factory.CreateViewModel(param);

            //Assert
            vm.Should().BeNull();
        }

        [Test]
        public void WHEN_orderResult_has_one_order_SHOULD_return_view_model()
        {
            //Arrange
            var factory = _container.CreateInstance<OrderHistoryViewModelFactory>();
            var customerId = Guid.NewGuid().ToString();
            var orderId = GetRandom.String(6);
            //Act
            var param = new GetOrderHistoryViewModelParam
            {
                CultureInfo = new CultureInfo("en-US"),
                Page = 1,
                OrderResult = new OrderQueryResult
                {
                    Results = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ShipmentItems = new List<ShipmentItem>
                            {
                                new ShipmentItem()
                            },
                            CustomerId = customerId,
                            Id = orderId,
                            OrderStatus = "InProgress"
                        }
                    },
                    TotalCount = 1
                },
                OrderStatuses = new Dictionary<string, string>
                {
                    {"InProgress", "In Progress"}
                },
                OrderDetailBaseUrl = GetRandom.String(32)
            };
            var vm = factory.CreateViewModel(param);

            //Assert
            vm.Should().NotBeNull();
            vm.Orders.Should().NotBeNull();
            var orderVm = vm.Orders.FirstOrDefault();
            orderVm.OrderInfos.CustomerId.ShouldBeEquivalentTo(customerId);
            orderVm.OrderInfos.Id.ShouldBeEquivalentTo(orderId);
            orderVm.OrderInfos.OrderStatus.ShouldBeEquivalentTo("In Progress");
        }

        [Test]
        [TestCase(1, 1, null, null, 1)]
        [TestCase(15, 1, null, 2, 2)]
        [TestCase(15, 2, 1, null, 2)]
        [TestCase(25, 2, 1, 3, 3)]
        public void WHEN_orderResult_has_n_order_SHOULD_return_right_pagination(int totalOrders, int page,
             int? expectedPreviousPageNumber, int? expectedNextPageNumber, int expectedPageCount)
        {
            //Arrange
            var factory = _container.CreateInstance<OrderHistoryViewModelFactory>();
            var customerId = Guid.NewGuid().ToString();

            //Act
            var param = new GetOrderHistoryViewModelParam
            {
                CultureInfo = new CultureInfo("en-US"),
                Page = page,
                OrderResult = new OrderQueryResult
                {
                    Results = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ShipmentItems = new List<ShipmentItem>(),
                            CustomerId = customerId,
                            OrderStatus = "test"
                        }
                    },
                    TotalCount = totalOrders
                },
                OrderStatuses = new Dictionary<string, string>
                {
                    {"test", "test"}
                },
                OrderDetailBaseUrl = GetRandom.String(32)
            };
            var vm = factory.CreateViewModel(param);

            //Assert
            vm.Should().NotBeNull();
            vm.Pagination.Should().NotBeNull();
            vm.Pagination.NextPage.PageNumber.ShouldBeEquivalentTo(expectedNextPageNumber);
            vm.Pagination.PreviousPage.PageNumber.ShouldBeEquivalentTo(expectedPreviousPageNumber);
            vm.Pagination.Pages.Count().ShouldBeEquivalentTo(expectedPageCount);
            vm.Pagination.Pages.FirstOrDefault(x => x.IsCurrentPage).PageNumber.ShouldBeEquivalentTo(page);
        }
    }
}
