using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Providers.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Tests.Providers
{
    public class EditingOrderProvider_IsOrderEditable
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


            //Act
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
            var result = await provider.IsOrderEditable(order).ConfigureAwait(false);
            //Assert
            result.Should().Be(expectedIsOrderEditable);
        }

        protected Overture.ServiceModel.Orders.Order CreateOrderWithShipments(List<Shipment> shipments)
        {
            return new Overture.ServiceModel.Orders.Order
            {
                OrderStatus = "InProgress",
                Cart = new Overture.ServiceModel.Orders.Cart
                {
                    Total = 100,
                    Shipments = shipments
                }
            };
        }

    }
}
