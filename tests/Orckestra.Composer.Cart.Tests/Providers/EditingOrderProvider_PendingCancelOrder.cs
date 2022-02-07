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
            var order = new Order
            {
                OrderStatus = orderStatus
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
            var order = new Order
            {
                OrderStatus = "New"
            };

            var orderFulfillmentState = new OrderFulfillmentState()
            {
                IsCancelable = isCancelable,
                IsProcessing = isProcessing
            };

            _container.GetMock<IOrderRepository>()
                .Setup(r => r.GetOrderFulfillmentStateAsync(It.IsAny<GetOrderFulfillmentStateParam>()))
                .ReturnsAsync(orderFulfillmentState);

            var result = await provider.PendingCancel(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(pendingCancel);
        }
    }
}
