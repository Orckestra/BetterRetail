using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Overture.ServiceModel.Requests.Orders;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Tests.Repositories.Order
{
    public class OrderRepositoryGetOrderAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IComposerOvertureClient>(MockBehavior.Strict));
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var order = new Overture.ServiceModel.Orders.Order
            {
                OrderNumber = GetRandom.String(10)
            };

            _container.GetMock<IComposerOvertureClient>()
                .Setup(r => r.SendAsync(It.IsAny<GetOrderByNumberRequest>()))
                .ReturnsAsync(order);

            var orderRepository = _container.CreateInstance<OrderRepository>();

            //Act
            var param = new GetCustomerOrderParam
            {
                CustomerId = Guid.NewGuid(),
                OrderNumber = GetRandom.String(10),
                Scope = GetRandom.String(32),
            };

            var result = await orderRepository.GetOrderAsync(param).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.OrderNumber.Should().BeEquivalentTo(order.OrderNumber);
        }
    }
}
