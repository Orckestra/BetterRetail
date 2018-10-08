using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.Repositories.Order;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Tests.Repositories.Order
{
    public class OrderRepositoryGetShipmentNotesAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOvertureClient>(MockBehavior.Strict));
        }

        [Test]
        public async void WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var notes = new List<Note>
            {
                new Note()
            };

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.IsAny<GetShipmentNotesRequest>()))
                .ReturnsAsync(notes);

            var orderRepository = _container.CreateInstance<OrderRepository>();

            //Act
            var param = new GetShipmentNotesParam
            {
                ShipmentId = Guid.NewGuid(),
                Scope = GetRandom.String(32),
            };

            var result = await orderRepository.GetShipmentNotesAsync(param).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Count.ShouldBeEquivalentTo(1);
        }
    }
}
