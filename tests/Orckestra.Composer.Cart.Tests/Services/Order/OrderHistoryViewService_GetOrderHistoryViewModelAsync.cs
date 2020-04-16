using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Cart.Services.Order;
using Orckestra.Composer.Cart.ViewModels.Order;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel.Orders;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Tests.Services.Order
{
    public class OrderHistoryViewServiceGetOrderHistoryViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOrderHistoryViewModelFactory>(MockBehavior.Strict));
            _container.Use(new Mock<IOrderRepository>(MockBehavior.Strict));
            _container.Use(new Mock<ILookupService>(MockBehavior.Strict));
            _container.Use(new Mock<IOrderUrlProvider>(MockBehavior.Strict));
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var orderStatuses = new Dictionary<string, string>();

            var orderHistoryViewModelService = _container.CreateInstance<OrderHistoryViewService>();

            _container.GetMock<ILookupService>()
                .Setup(r => r.GetLookupDisplayNamesAsync(It.IsAny<GetLookupDisplayNamesParam>()))
                .ReturnsAsync(orderStatuses);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetCustomerOrdersAsync(It.IsAny<GetCustomerOrdersParam>()))
                .ReturnsAsync(new OrderQueryResult
                {
                    Results = new List<OrderItem>()
                });

            _container.GetMock<IOrderHistoryViewModelFactory>()
                .Setup(r => r.CreateViewModel(It.IsAny<GetOrderHistoryViewModelParam>()))
                .Returns(new OrderHistoryViewModel());

            _container.GetMock<IOrderUrlProvider>()
               .Setup(r => r.GetOrderDetailsBaseUrl(It.IsAny<CultureInfo>()))
               .Returns(GetRandom.String(32));

            //Act
            var result =
                await orderHistoryViewModelService.GetOrderHistoryViewModelAsync(BuildGetCustomerOrdersParam()).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
        }

        private GetCustomerOrdersParam BuildGetCustomerOrdersParam()
        {
            var param = new GetCustomerOrdersParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = new CultureInfo("en-US"),
                OrderTense = OrderTense.CurrentOrders,
                Page = 1
            };
            return param;
        }
    }
}
