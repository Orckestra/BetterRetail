using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    public static class FulfillmentRepositoryMock
    {
        public static Mock<IFulfillmentMethodRepository> MockFulfillmentRepoWithRandomMethods()
        {
            Guid shipmentId = GetRandom.Guid();

            var fulfillmentRepoMock = new Mock<IFulfillmentMethodRepository>();
            fulfillmentRepoMock.Setup(m => m.GetCalculatedFulfillmentMethods(It.IsNotNull<GetShippingMethodsParam>()))
                .ReturnsAsync(new List<FulfillmentMethod>
                {
                    new FulfillmentMethod()
                    {
                        Id = GetRandom.Guid(),
                        Cost = GetRandom.Double(0, 30.0),
                        DisplayName = new LocalizedString(),
                        ExpectedDeliveryDate = GetRandom.DateTimeFrom(DateTime.Now),
                        FulfillmentMethodType = FulfillmentMethodType.Shipping,
                        Name = GetRandom.String(12),
                        ShipmentId = shipmentId,
                        ShippingProviderId = GetRandom.Guid(),
                        TaxCategory = GetRandom.String(12),
                        PropertyBag = new PropertyBag()
                    },
                    new FulfillmentMethod()
                    {
                        Id = GetRandom.Guid(),
                        Cost = GetRandom.Double(0, 30.0),
                        DisplayName = new LocalizedString(),
                        ExpectedDeliveryDate = GetRandom.DateTimeFrom(DateTime.Now),
                        FulfillmentMethodType = FulfillmentMethodType.Shipping,
                        Name = GetRandom.String(12),
                        ShipmentId = shipmentId,
                        ShippingProviderId = GetRandom.Guid(),
                        TaxCategory = GetRandom.String(12),
                        PropertyBag = new PropertyBag()
                    },
                    new FulfillmentMethod()
                    {
                        Id = GetRandom.Guid(),
                        Cost = GetRandom.Double(0, 30.0),
                        DisplayName = new LocalizedString(),
                        ExpectedDeliveryDate = GetRandom.DateTimeFrom(DateTime.Now),
                        FulfillmentMethodType = FulfillmentMethodType.Shipping,
                        Name = GetRandom.String(12),
                        ShipmentId = shipmentId,
                        ShippingProviderId = GetRandom.Guid(),
                        TaxCategory = GetRandom.String(12),
                        PropertyBag = new PropertyBag()
                    }
                });

            return fulfillmentRepoMock;
        }
    }
}
