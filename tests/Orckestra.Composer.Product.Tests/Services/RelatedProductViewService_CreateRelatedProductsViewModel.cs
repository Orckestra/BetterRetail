using System;
using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class RelatedProductViewService_CreateRelatedProductsViewModel : BaseTest
    {
        [Test]
        public void WHEN_Passed_Two_Products_SHOULD_Return_Two_ViewModels()
        {
            var vmMapper = Container.GetMock<IViewModelMapper>();
            vmMapper.Setup(
                m => m.MapTo<RelatedProductViewModel>(It.IsAny<Overture.ServiceModel.Products.Product>(), It.IsAny<CultureInfo>(), "CAD")).Returns(new RelatedProductViewModel());
            var service = Container.CreateInstance<RelatedProductViewServiceProxy>();

            var result = service.CreateRelatedProductsViewModelProxy(CreateParam());

            result.Products.Count.Should().Be(2);
        }

        private CreateRelatedProductViewModelParam CreateParam()
        {
            var param = new CreateRelatedProductViewModelParam();
            param.ProductsWithVariant = new List<ProductWithVariant>
            {
                new ProductWithVariant {Product = RelatedProductsViewServiceTestHelper.Product, Variant = new Variant
                {
                    Id = "1"
                }},
                new ProductWithVariant {Product = RelatedProductsViewServiceTestHelper.Product, Variant = new Variant
                {
                    Id = "2"
                }},
            };
            param.CultureInfo = new CultureInfo("en-CA");
            param.BaseUrl= new Uri("http://httpcolonslashslash.com");
            param.Prices = new List<ProductPrice>
            {
                new ProductPrice
                {
                    DefaultPrice = 99m,
                    ProductId = "1",
                    Pricing = new ProductPriceEntry {Price = 99m}
                }
            };
            param.Images = new List<ProductMainImage>
            {
                new ProductMainImage
                {
                    ProductId = "1",
                    VariantId = "1",
                    ImageUrl = GetRandom.WwwUrl(),
                    FallbackImageUrl = GetRandom.WwwUrl()
                },
                new ProductMainImage
                {
                    ProductId = "1",
                    VariantId = "2",
                    ImageUrl = GetRandom.WwwUrl(),
                    FallbackImageUrl = GetRandom.WwwUrl()
                }
            };
            return param;
        }
    }
}
