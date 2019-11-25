using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Cart.Services;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class LineItemService_GetInvalidLineItems
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            Container.GetMock<ILineItemValidationProvider>()
                .Setup(m => m.ValidateLineItem(It.IsNotNull<ProcessedCart>(), It.IsNotNull<LineItem>()))
                .Returns(new Func<ProcessedCart, LineItem, bool>((c, li) => GetRandom.Boolean()));
        }

        [Test]
        public void WHEN_cart_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            ProcessedCart cart = null;
            var sut = Container.CreateInstance<LineItemService>();

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => sut.GetInvalidLineItems(cart));

            //Assert
            exception.Should().NotBeNull();
            exception.ParamName.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void WHEN_cart_is_ok_SHOULD_call_lineItemvalidationprovider()
        {
            //Arrange
            var cart = new ProcessedCart
            {
                Shipments = new List<Shipment>
                {
                    new Shipment
                    {
                        LineItems = new List<LineItem>
                        {
                            new LineItem(),
                            new LineItem(),
                            new LineItem()
                        }
                    }
                }
            };
            var sut = Container.CreateInstance<LineItemService>();

            //Act
            var lineItems = sut.GetInvalidLineItems(cart);

            //Assert
            Container.GetMock<ILineItemValidationProvider>().Verify(m => m.ValidateLineItem(It.IsNotNull<ProcessedCart>(), It.IsNotNull<LineItem>()));
        }
    }
}
