using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Providers.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Tests.Providers
{
    public class EditingOrderProvider_IsOrderEditable
    {
        private AutoMocker _container;
        private Guid _customerId;
        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _customerId = Guid.NewGuid();

            var contextStub = new Mock<IComposerContext>();
            contextStub.SetupGet(mock => mock.Scope).Returns("Global");
            contextStub.SetupGet(mock => mock.CustomerId).Returns(_customerId);
            contextStub.SetupGet(mock => mock.IsAuthenticated).Returns(true);
            _container.Use(contextStub);
        }

        [Test]
        public async Task WHEN_order_is_Canceled_SHOULD_return_False()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var cartShipmentStatuses = new string[] { "Pending" };
            var shipmentList = cartShipmentStatuses?.Select(status => new Shipment()
            {
                Status = status,
                Address = new Address(),
                FulfillmentMethod = new FulfillmentMethod
                {
                    Cost = GetRandom.Double()
                },
                Taxes = new List<Tax>()
            }).ToList();

            var order = CreateOrderWithShipments(shipmentList);
            order.OrderStatus = Constants.OrderStatus.Canceled;

            //Act
            var result = await provider.CanEdit(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(false);
        }

        [Test]
        public async Task WHEN_order_customer_and_context_customer_are_different_SHOULD_return_False()
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var cartShipmentStatuses = new string[] { "Pending" };
            var shipmentList = cartShipmentStatuses?.Select(status => new Shipment()
            {
                Status = status,
                Address = new Address(),
                FulfillmentMethod = new FulfillmentMethod
                {
                    Cost = GetRandom.Double()
                },
                Taxes = new List<Tax>()
            }).ToList();

            var order = CreateOrderWithShipments(shipmentList);
            order.CustomerId = Guid.NewGuid().ToString();

            //Act
            var result = await provider.CanEdit(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(false);
        }

        [Test]
        public async Task WHEN_customer_is_not_authenticated_SHOULD_return_False()
        {
            //Setup
            _container.GetMock<IComposerContext>().SetupGet(mock => mock.IsAuthenticated).Returns(false);

            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            var cartShipmentStatuses = new string[] { "Pending" };
            var shipmentList = cartShipmentStatuses?.Select(status => new Shipment()
            {
                Status = status,
                Address = new Address(),
                FulfillmentMethod = new FulfillmentMethod
                {
                    Cost = GetRandom.Double()
                },
                Taxes = new List<Tax>()
            }).ToList();

            var order = CreateOrderWithShipments(shipmentList);

            //Act
            var result = await provider.CanEdit(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(false);
        }

        [Test]
        [TestCase("New|Pending", new string[] { "New" }, true)]
        [TestCase("New|Pending", new string[] { "Pending" }, true)]
        [TestCase("New|Pending", new string[] { "New", "Pending" }, true)]
        [TestCase("New|Pending", new string[] { "Canceled" }, false)]
        [TestCase("New|Pending", new string[] { "New,Canceled" }, false)]
        [TestCase(null, new string[] { "New", "Canceled" }, false)]
        [TestCase(null, null, false)]
        [TestCase("New|Pending", null, false)]
        public async Task WHEN_order_with_provided_shipment_statuses_SHOULD_return_correct_IsOrderEditableAsync(string editableShipmentStates,
         string[] cartShipmentStatuses,
         bool expectedIsOrderEditable)
        {
            //Arrange
            var provider = _container.CreateInstance<EditingOrderProvider>();

            _container.GetMock<IOrderRepository>()
            .Setup(r => r.GetOrderSettings(It.IsAny<string>())).ReturnsAsync(new OrderSettings()
            {
                EditableShipmentStates = editableShipmentStates
            });

            var shipmentList = cartShipmentStatuses?.Select(status => new Shipment()
            {
                Status = status,
                Address = new Address(),
                FulfillmentMethod = new FulfillmentMethod
                {
                    Cost = GetRandom.Double()
                },
                Taxes = new List<Tax>()
            }).ToList();

            var order = CreateOrderWithShipments(shipmentList);

            //Act
            var result = await provider.CanEdit(order).ConfigureAwait(false);

            //Assert
            result.Should().Be(expectedIsOrderEditable);
        }

        protected Order CreateOrderWithShipments(List<Shipment> shipments)
        {
            return new Order
            {
                OrderStatus = "InProgress",
                CustomerId = _customerId.ToString(),
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Total = 100,
                    Shipments = shipments
                }
            };
        }
    }
}
