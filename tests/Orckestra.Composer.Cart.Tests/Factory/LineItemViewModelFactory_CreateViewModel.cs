using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Factory
{
    // ReSharper disable once InconsistentNaming
    public class LineItemViewModelFactory_CreateViewModel
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
            var mapper = Container.GetMock<IViewModelMapper>();
            mapper.Setup(m => m.MapTo<LineItemDetailViewModel>(It.IsAny<LineItem>(), It.IsAny<CultureInfo>())).Returns<LineItem, CultureInfo>(
                (li, ci) => new LineItemDetailViewModel { ListPrice = li.CurrentPrice.GetValueOrDefault().ToString(CultureInfo.InvariantCulture), DefaultListPrice = li.DefaultPrice.GetValueOrDefault().ToString(CultureInfo.InvariantCulture) });
            Container.GetMock<ILocalizationProvider>()
                .Setup(m => m.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                .Returns("{0:C}");
        }

        [Test]
        public void WHEN_Product_Is_Not_On_Sale_IsPriceDiscounted_SHOULD_Be_False()
        {

            var factory = Container.CreateInstance<LineItemViewModelFactory>();

            var param = new CreateListOfLineItemDetailViewModelParam()
            {
                Cart = new ProcessedCart(),
                LineItems = new List<LineItem> {new LineItem {DefaultPrice = 9.99m, CurrentPrice = 9.99m}},
                CultureInfo = new CultureInfo("en-CA"),
                ImageInfo = new ProductImageInfo {ImageUrls = new List<ProductMainImage>()},
                BaseUrl = "http://orckestra.com/"
            };

            var viewModels = factory.CreateViewModel(param);

            var firstVm = viewModels.First();
            firstVm.IsPriceDiscounted.Should().BeFalse();
        }

        [Test]
        public void WHEN_DiscountAmount_Is_Positive_IsPriceDiscounted_SHOULD_Be_True()
        {
            var factory = Container.CreateInstance<LineItemViewModelFactory>();

            var param = new CreateListOfLineItemDetailViewModelParam()
            {
                Cart = new ProcessedCart(),
                LineItems = new List<LineItem> { new LineItem { DiscountAmount = 10m } },
                CultureInfo = new CultureInfo("en-CA"),
                ImageInfo = new ProductImageInfo { ImageUrls = new List<ProductMainImage>() },
                BaseUrl = "http://orckestra.com/"
            };
            var viewModels = factory.CreateViewModel(param);

            var firstVm = viewModels.First();
            firstVm.IsPriceDiscounted.Should().BeTrue();
        }

        [Test]
        public void WHEN_DiscountAmount_Is_Zero_IsPriceDiscounted_SHOULD_Be_False()
        {
            var factory = Container.CreateInstance<LineItemViewModelFactory>();

            var param = new CreateListOfLineItemDetailViewModelParam()
            {
                Cart = new ProcessedCart(),
                LineItems = new List < LineItem > { new LineItem { DiscountAmount = 0 } },
                CultureInfo = new CultureInfo("en-CA"),
                ImageInfo = new ProductImageInfo { ImageUrls = new List<ProductMainImage>() },
                BaseUrl = "http://orckestra.com/"
            };

            var viewModels = factory.CreateViewModel(param);

            var firstVm = viewModels.First();
            firstVm.IsPriceDiscounted.Should().BeFalse();
        }

        [Test]
        public void WHEN_ListPrice_Is_Less_Than_DefaultListPrice_IsOnSale_SHOULD_Be_True()
        {
            var factory = Container.CreateInstance<LineItemViewModelFactory>();
            var param = new CreateListOfLineItemDetailViewModelParam()
            {
                Cart = new ProcessedCart(),
                LineItems = new List<LineItem> {new LineItem {DefaultPrice = 9.99m, CurrentPrice = 5m}},
                CultureInfo = new CultureInfo("en-CA"),
                ImageInfo = new ProductImageInfo {ImageUrls = new List<ProductMainImage>()},
                BaseUrl = "http://orckestra.com/"
            };
            var viewModels = factory.CreateViewModel(param);

            var firstVm = viewModels.First();

            firstVm.IsOnSale.Should().BeTrue();
        }

        [Test]
        public void WHEN_ListPrice_Is_Greater_Than_DefaultListPrice_IsOnSale_SHOULD_Be_False()
        {

            var factory = Container.CreateInstance<LineItemViewModelFactory>();

            var param = new CreateListOfLineItemDetailViewModelParam()
            {
                Cart = new ProcessedCart(),
                LineItems = new List<LineItem> { new LineItem { DefaultPrice = 9.99m, CurrentPrice = 9.99m } },
                CultureInfo = new CultureInfo("en-CA"),
                ImageInfo = new ProductImageInfo { ImageUrls = new List<ProductMainImage>() },
                BaseUrl = "http://orckestra.com/"
            };

            var viewModels = factory.CreateViewModel(param);

            var firstVm = viewModels.First();
            firstVm.IsOnSale.Should().BeFalse();
        }

        [Test]
        public void WHEN_cart_has_lineItem_SHOULD_map_first_shipment_vm()
        {

            //Arrange
            Container.Use(ViewModelMapperFactory.CreateFake(typeof(LineItemDetailViewModel).Assembly));


            var lineItem = new LineItem
            {
                Quantity = GetRandom.Int(),
                ProductId = GetRandom.String(32),
                VariantId = GetRandom.String(32),
                Id = Guid.NewGuid(),
                Total = GetRandom.Decimal(),
                DefaultPrice = GetRandom.PositiveDecimal(2000000),
                CurrentPrice = new decimal()
            };

            var param = new CreateListOfLineItemDetailViewModelParam()
            {
                Cart = new ProcessedCart(),
                LineItems = new List<LineItem> { lineItem },
                CultureInfo = new CultureInfo("en-CA"),
                ImageInfo = new ProductImageInfo { ImageUrls = new List<ProductMainImage>() },
                BaseUrl = "http://orckestra.com/"
            };


            var sut = Container.CreateInstance<LineItemViewModelFactory>();

            //Act
            var vm = sut.CreateViewModel(param);

            //Assert
            vm.Should().NotBeNullOrEmpty();
            vm.Should().HaveSameCount(param.LineItems);
            var collection = vm.ToList();

            for (int i = 0; i < collection.Count; i++)
            {
                var liVm = collection[i];

                liVm.Should().NotBeNull();
                liVm.Id.Should().Be(lineItem.Id);
                liVm.ProductId.Should().Be(lineItem.ProductId);
                liVm.VariantId.Should().Be(lineItem.VariantId);
                liVm.ListPrice.Should().NotBeNullOrWhiteSpace();
                liVm.DefaultListPrice.Should().NotBeNullOrWhiteSpace();
                liVm.Quantity.Should().Be(lineItem.Quantity);
                liVm.Total.Should().NotBeNullOrWhiteSpace();

                liVm.ProductSummary.Should().NotBeNull();
                liVm.ProductSummary.DisplayName.Should().Be(lineItem.ProductSummary.DisplayName);
            }
        }
    }
}