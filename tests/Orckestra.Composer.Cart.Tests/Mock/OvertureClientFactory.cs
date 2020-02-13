using System;
using System.Collections.Generic;
using Moq;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.LineItems;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Payments;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Shipments;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class OvertureClientFactory
    {
        internal static Mock<IOvertureClient> Create()
        {
            ProcessedCart dummyCart = new ProcessedCart();
            List<CartSummary> dummyCartListSummary = new List<CartSummary> { new CartSummary() };
            var dummyOrder = new Order();

            var overtureClient = new Mock<IOvertureClient>();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetCartsByCustomerIdRequest>()))
                          .ReturnsAsync(dummyCartListSummary)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetCartRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<AddLineItemRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<AddPaymentRequest>()))
                        .ReturnsAsync(dummyCart)
                        .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<RemoveLineItemRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<UpdateLineItemRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<UpdateShipmentRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<CompleteCheckoutRequest>()))
                .ReturnsAsync(dummyOrder)
                .Verifiable();

            return overtureClient;
        }

        internal static Mock<IOvertureClient> CreateMockWithValue(ProcessedCart cart)
        {
            var overtureClient = new Mock<IOvertureClient>();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetCartRequest>()))
                          .ReturnsAsync(cart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<AddLineItemRequest>()))
                          .ReturnsAsync(cart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<AddPaymentRequest>()))
                        .ReturnsAsync(cart)
                        .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<RemoveLineItemRequest>()))
                          .ReturnsAsync(cart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<UpdateLineItemRequest>()))
                          .ReturnsAsync(cart)
                          .Verifiable();
                          
            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<UpdateShipmentRequest>()))
                          .ReturnsAsync(cart)
                          .Verifiable();

            return overtureClient;
        }

        internal static Mock<IOvertureClient> CreateWithNullValues()
        {
            ProcessedCart dummyCart = new ProcessedCart
            {
                AdditionalFeeTotal = null,
                Name = null,
                AdjustmentTotal = null,
                BillingCurrency = null,
                CartType = null,
                PropertyBag = null,
                Coupons = null,
                CreatedBy = null,
                CultureName = null,
                Customer = null,
                DiscountTotal = null,
                FulfillmentCost = null,
                LastModifiedBy = null,
                MerchandiseTotal = null,
                OrderLocation = null,
                OriginalPromotions = null,
                ScopeId = null,
                Shipments = null,
                Status = null,
                SubTotal = null,
                TaxTotal = null,
                Total = null,
            };

            List<CartSummary> dummyCartListSummary = new List<CartSummary> { new CartSummary
            {
                AdditionalFeeTotal = null,
                Name = null,
                Total = null,
                MerchandiseTotal = null,
                ScopeId = null,
                TaxTotal = null,
                BillingCurrency = null,
                DiscountTotal = null,
                SubTotal = null,
                PropertyBag = null,
                DiscountAmount = null,
                ShipmentSummaries = null,
                ShippingTotal = null,
            } };

            Order dummyOrder = new Order
            {
                Cart = null,
                CustomerId = null,
                ScopeId = null,
                Created = DateTime.MinValue,
                Id = null,
                Source = null,
                OrderNumber = null,
                CreatedBy = null,
                OrderStatus = null,
                LastModified = DateTime.MinValue,
                LastModifiedBy = null,
                CustomerName = null,
                ItemCount = -1
            };


            var overtureClient = new Mock<IOvertureClient>();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetCartsByCustomerIdRequest>()))
                          .ReturnsAsync(dummyCartListSummary)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<GetCartRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<AddLineItemRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client=> client.SendAsync(It.IsNotNull<AddPaymentRequest>()))
                        .ReturnsAsync(dummyCart)
                        .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<RemoveLineItemRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<UpdateLineItemRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<UpdateShipmentRequest>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            overtureClient.Setup(client => client.SendAsync(It.IsNotNull<CompleteCheckoutRequest>()))
                .ReturnsAsync(dummyOrder)
                .Verifiable();

            return overtureClient;
        }
    }
}
