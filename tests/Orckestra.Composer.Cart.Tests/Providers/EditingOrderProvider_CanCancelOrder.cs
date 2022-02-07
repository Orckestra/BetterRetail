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

namespace Orckestra.Composer.Cart.Tests.Providers
{
    public class EditingOrderProvider_CanCancelOrder
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
        [TestCase(true, "New")]
        [TestCase(false, "Canceled")]
        [TestCase(false, "Completed")]
        public async Task WHEN_order_is_InProgress_And_FulfillmentState_Is_Cancelable_SHOULD_return_False(bool orderFulfillmentStateIsProcessing, string orderFulfillmentStateStatus)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
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

            var result = await provider.CanCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(false);
        }

        [Test]
        [TestCase("Canceled", false)]
        [TestCase("Completed", false)]
        [TestCase("New", true)]
        public async Task When_Order_has_Status_SHOULD_return_correct_CanCancel(string orderStatus, bool canCancel)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
            var order = new Order
            {
                OrderStatus = orderStatus
            };
            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "InProgress", allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            var result = await provider.CanCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(canCancel);
        }

        [Test]
        public async Task WHEN_order_is_InProgress_And_FulfillmentState_Is_Cancelable_SHOULD_return_True()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
            var order = new Order
            {
                OrderStatus = "InProgress"
            };
            var allowedStatusChanges = new List<string>()
            {
                Constants.OrderStatus.Canceled
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "New", allowedStatusChanges);

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            var result = await provider.CanCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(true);
        }

        [Test]
        public async Task WHEN_order_is_InProgress_And_FulfillmentState_AllowedStatusChanges_Is__Not_Canceled_SHOULD_return_False()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
            var order = new Order
            {
                OrderStatus = "InProgress"
            };

            var orderFulfillmentState = CreateFulfillmentState(true, false, "New", new List<string>());

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            var result = await provider.CanCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(false);
        }

        [Test]
        public async Task WHEN_order_is_InProgress_And_FulfillmentStateIs_Not_Cancelable_SHOULD_return_False()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            //Act
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

            var result = await provider.CanCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(false);
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
    }
}
