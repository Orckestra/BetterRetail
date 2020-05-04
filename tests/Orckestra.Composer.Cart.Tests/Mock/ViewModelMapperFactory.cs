using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class ViewModelMapperFactory
    {
        internal static Mock<IViewModelMapper> Create()
        {
            CartProductSummaryViewModel dummyCartProduct = new CartProductSummaryViewModel
            {
                DisplayName = GetRandom.String(32),
            };

            TaxViewModel dummyTaxViewModel = new TaxViewModel
            {
                DisplayName = GetRandom.String(32),
                TaxTotal = GetRandom.Decimal(1.0m, 200.0m)
            };

            LineItemDetailViewModel dummyLineItem = new LineItemDetailViewModel
            {
                ImageUrl = GetRandom.WwwUrl(),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Total = GetRandom.PositiveDouble().ToString(CultureInfo.InvariantCulture),
                DefaultListPrice = GetRandom.PositiveDouble().ToString(CultureInfo.InvariantCulture),
                ListPrice = GetRandom.PositiveDouble().ToString(CultureInfo.InvariantCulture),
                ProductSummary = dummyCartProduct,
                FallbackImageUrl = GetRandom.WwwUrl(),
                IsOnSale = GetRandom.Boolean(),
                Quantity = GetRandom.PositiveInt(),
            };

            OrderSummaryViewModel dummyOrderSummaryViewModel = new OrderSummaryViewModel
            {
                Shipping = GetRandom.String(32),
                SubTotal = GetRandom.String(32),
                Total = GetRandom.String(32),
                DiscountTotal = GetRandom.String(32),
                Taxes = new List<TaxViewModel> { dummyTaxViewModel }
            };

            CartViewModel dummyCart = new CartViewModel
            {
                HomepageUrl = GetRandom.String(32),
                LineItemDetailViewModels = new List<LineItemDetailViewModel> { dummyLineItem },
                OrderSummary = dummyOrderSummaryViewModel,
                IsCartEmpty = GetRandom.Boolean(),
                TotalQuantity = GetRandom.PositiveInt(),
                LineItemCount = GetRandom.PositiveInt()
            };

            var rewardViewModel = new RewardViewModel()
            {
                Description = GetRandom.Phrase(40)
            };

            var couponViewModel = new CouponViewModel()
            {
                CouponCode = GetRandom.String(7),
                DisplayText = GetRandom.Phrase(35)
            };

            var viewModelMapper = new Mock<IViewModelMapper>();            

            viewModelMapper.Setup(
                mapper =>
                    mapper.MapTo<CartProductSummaryViewModel>(It.IsNotNull<CartProductSummary>(),
                        It.IsNotNull<CultureInfo>()))
                .Returns(dummyCartProduct)
            .Verifiable();

            viewModelMapper.Setup(mapper => mapper.MapTo<CartViewModel>(It.IsNotNull<Overture.ServiceModel.Orders.Cart>(), It.IsNotNull<CultureInfo>()))
                          .Returns(dummyCart)
                          .Verifiable();

            viewModelMapper.Setup(
                mapper => mapper.MapTo<LineItemDetailViewModel>(It.IsNotNull<LineItem>(), It.IsNotNull<CultureInfo>()))
                .Returns(dummyLineItem)
            .Verifiable();

            viewModelMapper.Setup(mapper => mapper.MapTo<TaxViewModel>(It.IsNotNull<Tax>(), It.IsNotNull<CultureInfo>()))
                .Returns(dummyTaxViewModel)
            .Verifiable();

            viewModelMapper.Setup(mapper=>mapper.MapTo<RewardViewModel>(It.IsNotNull<Reward>(), It.IsNotNull<CultureInfo>()))
                .Returns(rewardViewModel)
                .Verifiable();

            viewModelMapper.Setup(
                mapper => mapper.MapTo<CouponViewModel>(It.IsNotNull<Coupon>(), It.IsNotNull<CultureInfo>()))
                .Returns(couponViewModel)
            .Verifiable();

            return viewModelMapper;
        }

        internal static Mock<IViewModelMapper> CreateViewNullValues()
        {
            CartProductSummaryViewModel dummyCartProduct = new CartProductSummaryViewModel
            {
                DisplayName = null,
            };

            LineItemDetailViewModel dummyLineItem = new LineItemDetailViewModel
            {
                ImageUrl = null,
                ProductId = null,
                VariantId = null,
                Total = null,
                DefaultListPrice = null,
                ListPrice = null,
                ProductSummary = null,
                FallbackImageUrl = null,
                IsOnSale = GetRandom.Boolean(),
                Quantity = GetRandom.PositiveInt(),
            };

            TaxViewModel dummyTaxViewModel = new TaxViewModel
            {
                DisplayName = null,
                TaxTotal = null
            };

            OrderSummaryViewModel dummyOrderSummary = new OrderSummaryViewModel
            {
                Shipping = null,
                SubTotal = null,
                Taxes = null,
                Total = null,
                DiscountTotal = null
            };

            CartViewModel dummyCart = new CartViewModel
            {
                HomepageUrl = null,
                LineItemDetailViewModels = new List<LineItemDetailViewModel> { dummyLineItem },
                OrderSummary = null,
                IsCartEmpty = GetRandom.Boolean(),
                TotalQuantity = GetRandom.PositiveInt(),
                LineItemCount = GetRandom.PositiveInt()
            };

            RewardViewModel dummyReward = new RewardViewModel()
            {
                Description = null
            };

            CouponViewModel dummyCoupons = new CouponViewModel()
            {
                CouponCode = null,
                DisplayText = null
            };

            var viewModelMapper = new Mock<IViewModelMapper>();


            viewModelMapper.Setup(
                mapper =>
                    mapper.MapTo<CartProductSummaryViewModel>(It.IsNotNull<CartProductSummary>(),
                        It.IsNotNull<CultureInfo>()))
                .Returns(dummyCartProduct)
                            .Verifiable();

            viewModelMapper.Setup(
                mapper =>
                    mapper.MapTo<CartViewModel>(It.IsNotNull<Overture.ServiceModel.Orders.Cart>(),
                        It.IsNotNull<CultureInfo>()))
                .Returns(dummyCart)
                         .Verifiable();

            viewModelMapper.Setup(
                mapper => mapper.MapTo<LineItemDetailViewModel>(It.IsNotNull<LineItem>(), It.IsNotNull<CultureInfo>()))
                .Returns(dummyLineItem)
                          .Verifiable();

            viewModelMapper.Setup(
                mapper =>
                    mapper.MapTo<OrderSummaryViewModel>(It.IsNotNull<Overture.ServiceModel.Orders.Cart>(),
                        It.IsNotNull<CultureInfo>()))
                .Returns(dummyOrderSummary)
                          .Verifiable();

            viewModelMapper.Setup(mapper => mapper.MapTo<TaxViewModel>(It.IsNotNull<Tax>(), It.IsNotNull<CultureInfo>()))
                .Returns(dummyTaxViewModel)
                          .Verifiable();

            viewModelMapper.Setup(mapper => mapper.MapTo<RewardViewModel>(It.IsNotNull<Reward>(), It.IsNotNull<CultureInfo>()))
                .Returns(dummyReward)
                .Verifiable();

            viewModelMapper.Setup(mapper=>mapper.MapTo<CouponViewModel>(It.IsNotNull<Coupon>(), It.IsNotNull<CultureInfo>()))
                .Returns(dummyCoupons)
                .Verifiable();

            return viewModelMapper;
        }

        internal static IViewModelMapper CreateFake(params Assembly[] assemblies)
        {
            var registry = new ViewModelMetadataRegistry();

            foreach (var assembly in assemblies)
            {
                registry.LoadViewModelMetadataInAssemblyOf(assembly);
            }

            var formatterMock = new Mock<IViewModelPropertyFormatter>();
            formatterMock.Setup(m => m.Format(It.IsAny<object>(), It.IsNotNull<IPropertyMetadata>(), It.IsAny<CultureInfo>()))
                .Returns((object value, IPropertyMetadata meta, CultureInfo culture) => value?.ToString());

            var lookupServiceMock = new Mock<ILookupService>();
            var localizationProviderMock = new Mock<ILocalizationProvider>();

            var mapper = new ViewModelMapper(registry, formatterMock.Object, lookupServiceMock.Object, localizationProviderMock.Object);
            return mapper;
        }
    }
}
