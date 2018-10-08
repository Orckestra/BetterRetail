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
    internal static class WishListRepositoryFactory
    {
        internal static Mock<IWishListRepository> Create()
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

            var repository = new Mock<IWishListRepository>();

            repository.Setup(repo => repo.GetWishListAsync(It.IsNotNull<GetCartParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();
            repository.Setup(repo => repo.AddLineItemAsync(It.IsNotNull<AddLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            repository.Setup(repo => repo.RemoveLineItemAsync(It.IsNotNull<RemoveLineItemParam>()))
                          .ReturnsAsync(dummyCart)
                          .Verifiable();

            return repository;
        }

        internal static Mock<IWishListRepository> CreateWithNullValues()
        {
            var dummyCart = new ProcessedCart
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

            var repository = new Mock<IWishListRepository>();

            repository.Setup(repo => repo.GetWishListAsync(It.IsNotNull<GetCartParam>()))
                .ReturnsAsync(dummyCart)
                .Verifiable();
            repository.Setup(repo => repo.AddLineItemAsync(It.IsNotNull<AddLineItemParam>()))
                .ReturnsAsync(dummyCart)
                .Verifiable();

            repository.Setup(repo => repo.RemoveLineItemAsync(It.IsNotNull<RemoveLineItemParam>()))
                .ReturnsAsync(dummyCart)
                .Verifiable();

            return repository;
        }

        internal static Mock<IWishListRepository> CreateWithValues(ProcessedCart wishList)
        {
            var repository = new Mock<IWishListRepository>();

            repository.Setup(repo => repo.GetWishListAsync(It.IsNotNull<GetCartParam>()))
                          .ReturnsAsync(wishList)
                          .Verifiable();
            repository.Setup(repo => repo.AddLineItemAsync(It.IsNotNull<AddLineItemParam>()))
                          .ReturnsAsync(wishList)
                          .Verifiable();

            repository.Setup(repo => repo.RemoveLineItemAsync(It.IsNotNull<RemoveLineItemParam>()))
                          .ReturnsAsync(wishList)
                          .Verifiable();

            return repository;
        }

    }
}
