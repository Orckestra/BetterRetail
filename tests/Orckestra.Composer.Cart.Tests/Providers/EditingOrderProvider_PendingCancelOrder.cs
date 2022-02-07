using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Providers.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;
using System.Threading.Tasks;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Cart.Tests.Providers
{
    public class EditingOrderProvider_PendingCancelOrder
    {
        private AutoMocker _container;
        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var contextStub = new Mock<IComposerContext>();
            contextStub.SetupGet(mock => mock.Scope).Returns("Global");
            _container.Use(contextStub);
        }

        [Test]
        [TestCase("Canceled", false)]
        [TestCase("Completed", false)]
        [TestCase("New", true)]
        public async Task WHEN_order_is_InStatus_SHOULD__return_correct_PendingCancel(string orderStatus, bool pendingCancel)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
            var orderId = Guid.NewGuid().ToString();
            var order = new Order
            {
                OrderStatus = orderStatus,
                Id = orderId
            };

        var orderFulfillmentState = CreateOrderFulfillmentState(false, true, orderId);

        _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            var result = await provider.PendingCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(pendingCancel);
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, true)]
        public async Task WHEN_orderStatus_is_New_and_orderFulfillmentState_is_InState_SHOULD__return_correct_PendingCancel(bool isCancelable, bool isProcessing, bool pendingCancel)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
            var orderId = Guid.NewGuid().ToString();
            var order = new Order
            {
                OrderStatus = "New",
                Id = orderId
            };

            var orderFulfillmentState = CreateOrderFulfillmentState(isCancelable, isProcessing, orderId);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            var result = await provider.PendingCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(pendingCancel);
        }

        private OrderFulfillmentState CreateOrderFulfillmentState(bool isCancelable, bool isProcessing, string orderId)
        {
            var propertyBag = new Dictionary<string, object>();
            propertyBag.Add(Constants.DefaultOrderCancellationReason, DateTime.UtcNow);

            return new OrderFulfillmentState()
            {
                IsCancelable = isCancelable,
                IsProcessing = isProcessing,
                ShipmentFulfillmentStates = new List<ShipmentFulfillmentState>()
                {
                    new ShipmentFulfillmentState()
                    {
                        Messages = new List<ExecutionMessage>()
                        {
                            new ExecutionMessage()
                            {
                                MessageId = orderId,
                                PropertyBag = new PropertyBag(propertyBag)
                            }
                        }
                    }
                }
            };
        }

        [Test]
        public async Task WHEN_ShipmentFulfillmentStates_is_null_SHOULD_return_false_PendingCancel()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
            var orderId = Guid.NewGuid().ToString();
            var order = new Order
            {
                OrderStatus = "New",
                Id = orderId
            };

            var orderFulfillmentState = new OrderFulfillmentState()
            {
                IsCancelable = false,
                IsProcessing = true
            };

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            var result = await provider.PendingCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(false);
        }
    }
}
