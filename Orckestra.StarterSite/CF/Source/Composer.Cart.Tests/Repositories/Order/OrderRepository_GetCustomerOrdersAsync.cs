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
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Tests.Repositories.Order
{
    public class OrderRepositoryGetCustomerOrdersAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOvertureClient>(MockBehavior.Strict));
            _container.Use(new Mock<IFindOrdersRequestFactory>(MockBehavior.Strict));
        }

        [Test]
        public async void WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var findOrderRequest = new FindOrdersRequest();
            var orderQueryResult = new OrderQueryResult
            {
                TotalCount = 1,
                Results = new List<OrderItem>
                {
                    new OrderItem()
                }
            };
            var orderRepository = _container.CreateInstance<OrderRepository>();

            _container.GetMock<IFindOrdersRequestFactory>()
              .Setup(r => r.Create(It.IsAny<GetCustomerOrdersParam>()))
              .Returns(findOrderRequest);

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(findOrderRequest))
                .ReturnsAsync(orderQueryResult);

            //Act
            var result = await orderRepository.GetCustomerOrdersAsync(BuildGetCustomerOrdersParam()).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.TotalCount.ShouldBeEquivalentTo(1);
            result.Results.Should().NotBeNull();
        }

        [Test]
        public async void WHEN_result_is_null_SHOULD_return_null()
        {
            //Arrange
            var findOrderRequest = new FindOrdersRequest();
            var orderRepository = _container.CreateInstance<OrderRepository>();

            _container.GetMock<IFindOrdersRequestFactory>()
              .Setup(r => r.Create(It.IsAny<GetCustomerOrdersParam>()))
              .Returns(findOrderRequest);

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(findOrderRequest))
                .ReturnsAsync(null);

            //Act
            var result = await orderRepository.GetCustomerOrdersAsync(BuildGetCustomerOrdersParam()).ConfigureAwait(false);

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public async void WHEN_result_is_empty_SHOULD_return_null()
        {
            //Arrange
            var findOrderRequest = new FindOrdersRequest();
            var orderQueryResult = new OrderQueryResult
            {
                TotalCount = 0,
   
            };
            var orderRepository = _container.CreateInstance<OrderRepository>();

            _container.GetMock<IFindOrdersRequestFactory>()
              .Setup(r => r.Create(It.IsAny<GetCustomerOrdersParam>()))
              .Returns(findOrderRequest);

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(findOrderRequest))
                .ReturnsAsync(orderQueryResult);

            //Act
            var result = await orderRepository.GetCustomerOrdersAsync(BuildGetCustomerOrdersParam()).ConfigureAwait(false);

            //Assert
            result.Should().BeNull();
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
