using System;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Providers.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Orders.Fulfillment;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Cart.Tests.Providers
{
    public class EditingOrderProvider_GetOrderCancellationStatus
    {
        private AutoMocker _container;
        private Guid _currentCustomerId;
        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _currentCustomerId = Guid.NewGuid();

            var contextStub = new Mock<IComposerContext>();
            contextStub.SetupGet(mock => mock.Scope).Returns("Global");
            contextStub.SetupGet(mock => mock.CustomerId).Returns(_currentCustomerId);
            contextStub.SetupGet(mock => mock.IsAuthenticated).Returns(true);
            _container.Use(contextStub);
        }

        [Test]
        [TestCase(true, "New")]
        [TestCase(false, "Canceled")]
        [TestCase(false, "Completed")]
        public async Task WHEN_order_is_InProgress_And_FulfillmentState_Is_Cancelable_SHOULD_return_False(bool orderFulfillmentStateIsProcessing, string orderFulfillmentStateStatus)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var order = new Order
            {
                OrderStatus = "InProgress"
            };
            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };

            var orderFulfillmentState = CreateFulfillmentState(true, orderFulfillmentStateIsProcessing, orderFulfillmentStateStatus, allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CanCancel.Should().Be(false);
        }

        [Test]
        [TestCase("Canceled", false)]
        [TestCase("Completed", false)]
        [TestCase("New", true)]
        public async Task When_Order_has_Status_SHOULD_return_correct_CanCancel(string orderStatus, bool canCancel)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var order = new Order
            {
                OrderStatus = orderStatus,
                CustomerId = _currentCustomerId.ToString()
            };
            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "InProgress", allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CanCancel.Should().Be(canCancel);
        }

        [Test]
        public async Task WHEN_order_is_InProgress_And_FulfillmentState_Is_Cancelable_SHOULD_return_True()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var order = new Order
            {
                OrderStatus = "InProgress",
                CustomerId = _currentCustomerId.ToString()
            };

            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "New", allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CanCancel.Should().Be(true);
        }

        [Test]
        public async Task WHEN_order_customer_and_context_customer_are_different_SHOULD_return_False()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var order = new Order
            {
                OrderStatus = "InProgress",
                CustomerId = Guid.NewGuid().ToString()
            };
            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "New", allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CanCancel.Should().Be(false);
        }

        [Test]
        public async Task WHEN_customer_is_not_authenticated_SHOULD_return_False()
        {
            //Setup
            _container.GetMock<IComposerContext>().SetupGet(mock => mock.IsAuthenticated).Returns(false);

            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var order = new Order
            {
                OrderStatus = "InProgress",
                CustomerId = _currentCustomerId.ToString()
            };
            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "New", allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CanCancel.Should().Be(false);
        }

        [Test]
        public async Task WHEN_order_is_InProgress_And_FulfillmentState_AllowedStatusChanges_Is__Not_Canceled_SHOULD_return_False()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var order = new Order
            {
                OrderStatus = "InProgress"
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "New", new List<string>());

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CanCancel.Should().Be(false);
        }

        [Test]
        public async Task WHEN_order_is_InProgress_And_FulfillmentStateIs_Not_Cancelable_SHOULD_return_False()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var order = new Order
            {
                OrderStatus = "InProgress"
            };
            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };
            var orderFulfillmentState = CreateFulfillmentState(false, false, "New", allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CanCancel.Should().Be(false);
        }

        private OrderFulfillmentState CreateFulfillmentState(bool orderFulfillmentStateIsCancelable,
            bool orderFulfillmentStateIsProcessing,
            string orderFulfillmentStateStatus,
            List<string> allowedStatusChanges)
        {
            return new OrderFulfillmentState()
            {
                IsCancelable = orderFulfillmentStateIsCancelable,
                IsProcessing = orderFulfillmentStateIsProcessing,
                Status = orderFulfillmentStateStatus,
                ShipmentFulfillmentStates = new List<ShipmentFulfillmentState>()
                {
                    new ShipmentFulfillmentState()
                    {
                        AllowedStatusChanges = allowedStatusChanges
                    }
                }
            };
        }

        [Test]
        [TestCase("Canceled", false)]
        [TestCase("Completed", false)]
        [TestCase("New", true)]
        public async Task WHEN_order_is_InStatus_SHOULD__return_correct_PendingCancel(string orderStatus, bool pendingCancel)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var orderId = Guid.NewGuid().ToString();
            var order = new Order
            {
                OrderStatus = orderStatus,
                Id = orderId,
                CustomerId = _currentCustomerId.ToString()
            };

            var orderFulfillmentState = CreateOrderFulfillmentState(false, true, orderId);

            _container.GetMock<IOrderRepository>()
                    .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                    .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CancellationPending.Should().Be(pendingCancel);
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, true)]
        public async Task WHEN_orderStatus_is_New_and_orderFulfillmentState_is_InState_SHOULD__return_correct_PendingCancel(bool isCancelable, bool isProcessing, bool pendingCancel)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var orderId = Guid.NewGuid().ToString();
            var order = new Order
            {
                OrderStatus = "New",
                Id = orderId,
                CustomerId = _currentCustomerId.ToString()
            };

            var orderFulfillmentState = CreateOrderFulfillmentState(isCancelable, isProcessing, orderId);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CancellationPending.Should().Be(pendingCancel);
        }

        private OrderFulfillmentState CreateOrderFulfillmentState(bool isCancelable, bool isProcessing, string orderId)
        {
            var propertyBag = new Dictionary<string, object>();
            propertyBag.Add(Constants.RequestedOrderCancellationDatePropertyBagKey, DateTime.UtcNow);

            return new OrderFulfillmentState()
            {
                IsCancelable = isCancelable,
                IsProcessing = isProcessing,
                OrderId = Guid.Parse(orderId),
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
                        },
                        AllowedStatusChanges = new List<string>()
                    }
                }
            };
        }

        [Test]
        public async Task WHEN_ShipmentFulfillmentStates_is_null_SHOULD_return_false_PendingCancel()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

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

            //Act
            var result = await provider.GetCancellationStatus(order).ConfigureAwait(false);

            //Assert
            result.CancellationPending.Should().Be(false);
        }
    }
}
