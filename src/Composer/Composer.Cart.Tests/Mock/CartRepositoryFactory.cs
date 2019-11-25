using System;
using System.Collections.Generic;
using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Coupons;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class CartRepositoryFactory
    {
        internal static Mock<ICartRepository> Create()
        {
            var dummyCart = new ProcessedCart
            {
                CustomerId = Guid.NewGuid(),
                Shipments = new List<Shipment> { new Shipment() },
                BillingCurrency = GetRandom.String(1),
                CartType = GetRandom.String(1),
                Name = GetRandom.String(1),
                ScopeId = GetRandom.String(1),
                Status = GetRandom.String(1)
            };

            var dummyCartListSummary = new List<CartSummary> { new CartSummary() };
            var dummyOrder = new Order();

            var cartRepository = new Mock<ICartRepository>();

            cartRepository.Setup(repo => repo.GetCartsByCustomerIdAsync(It.IsNotNull<GetCartsByCustomerIdParam>()))
                          .ReturnsAsync(dummyCartListSummary)
                          .Verifiable();

            cartRepository.Setup(repo => repo.GetCartAsync(It.IsNotNull<GetCartParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.AddLineItemAsync(It.IsNotNull<AddLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.RemoveLineItemAsync(It.IsNotNull<RemoveLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.UpdateLineItemAsync(It.IsNotNull<UpdateLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.UpdateCartAsync(It.IsNotNull<UpdateCartParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.CompleteCheckoutAsync(It.IsNotNull<CompleteCheckoutParam>()))
                .ReturnsAsync(dummyOrder)
                .Verifiable();

            return cartRepository;
        }

        internal static Mock<ICartRepository> CreateWithNullValues()
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
                Total = null
            };

            List<CartSummary> dummyCartListSummary = new List<CartSummary> 
            { 
                new CartSummary
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
                    ShippingTotal = null
                } 
            };

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

            var cartRepository = new Mock<ICartRepository>();

            cartRepository.Setup(repo => repo.GetCartsByCustomerIdAsync(It.IsNotNull<GetCartsByCustomerIdParam>()))
                          .ReturnsAsync(dummyCartListSummary)
                          .Verifiable();

            cartRepository.Setup(repo => repo.GetCartAsync(It.IsNotNull<GetCartParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.AddLineItemAsync(It.IsNotNull<AddLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.RemoveLineItemAsync(It.IsNotNull<RemoveLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.UpdateLineItemAsync(It.IsNotNull<UpdateLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.UpdateCartAsync(It.IsNotNull<UpdateCartParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            cartRepository.Setup(repo => repo.CompleteCheckoutAsync(It.IsNotNull<CompleteCheckoutParam>()))
                          .ReturnsAsync(dummyOrder)
                          .Verifiable();

            return cartRepository;
        }

        internal static ProcessedCart CreateCartBasedOnAddCouponRequest(AddCouponRequest request, CouponState state)
        {
            var cart = new ProcessedCart
            {
                Name = request.CartName,
                CultureName = request.CultureName,
                Coupons = new List<Coupon>()
                {
                    new Coupon()
                    {
                        CouponCode = request.CouponCode,
                        CouponState = state
                    }
                },
                CustomerId = request.CustomerId,
                ScopeId = request.ScopeId,
                Shipments = new List<Shipment>()
            };

            return cart;
        }
    }
}
