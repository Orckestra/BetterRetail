using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.Tests.Factories;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    public class ProductViewModelFactory_BuildImages : ProductViewModelFactoryTestBase
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();
        }

        [Test]
        public void WHEN_There_Are_Thumbnail_Images_First_SHOULD_Be_Selected()
        {
            Container.GetMock<IViewModelMapper>()
                .Setup(
                    m => m.MapTo<ProductDetailImageViewModel>(It.IsAny<AllProductImages>(), It.IsAny<CultureInfo>()))
                .Returns(() => new ProductDetailImageViewModel());

            var service = Container.CreateInstance<ProductViewModelFactoryProxy>();
            var productImages = CreateAllProductImages();

            var images = service.BuildImagesProxy("1", "2", "Men's Family Briefs Red", productImages, new CultureInfo("en-CA"));

            images.First().Selected.Should().BeTrue();
            images.Skip(1).First().Selected.Should().BeFalse();
        }

        private IEnumerable<AllProductImages> CreateAllProductImages()
        {
            return new List<AllProductImages> { new AllProductImages { ImageUrl = "http://i0.kym-cdn.com/photos/images/original/000/581/722/7bc.jpg", ProductId = "1", SequenceNumber = 1, ThumbnailUrl = "http://i0.kym-cdn.com/photos/images/masonry/000/581/722/7bc.jpg", ProductZoomImageUrl = "http://i0.kym-cdn.com/photos/images/original/000/581/722/7bc.jpg", VariantId = "2" }, new AllProductImages { ImageUrl = "http://i0.kym-cdn.com/photos/images/original/000/581/722/7bc.jpg", ProductId = "1", SequenceNumber = 2, ThumbnailUrl = "http://i0.kym-cdn.com/photos/images/masonry/000/581/722/7bc.jpg", ProductZoomImageUrl = "http://i0.kym-cdn.com/photos/images/original/000/581/722/7bc.jpg", VariantId = "2"} };

        } 
        private class ProductViewModelFactoryProxy : ProductViewModelFactory
        {
            public ProductViewModelFactoryProxy(IViewModelMapper viewModelMapper,
                IProductRepository productRepository,
                IDamProvider damProvider,
                ILocalizationProvider localizationProvider,
                ILookupService lookupService,
                IProductUrlProvider productUrlProvider,
                IScopeViewService scopeViewService,
                IRecurringOrdersRepository recurringOrdersRepository,
                IRecurringOrderProgramViewModelFactory recurringOrderProgramViewModelFactory,
                IRecurringOrdersSettings recurringOrdersSettings,
                IProductSpecificationsViewService productSpecificationsViewService,
                IMyAccountUrlProvider myAccountUrlProvider)
            
                : base(
                viewModelMapper, 
                productRepository, 
                damProvider, 
                localizationProvider, 
                lookupService, 
                productUrlProvider,
                scopeViewService,
                recurringOrdersRepository,
                recurringOrderProgramViewModelFactory,
                recurringOrdersSettings,
                productSpecificationsViewService,
                myAccountUrlProvider)
            {
            }

            public IEnumerable<ProductDetailImageViewModel> BuildImagesProxy(
                string productId, 
                string variantId, 
                string productDisplayName,
                IEnumerable<AllProductImages> productImages, 
                CultureInfo cultureInfo)
            {
                return BuildImages(productId, variantId, productDisplayName, productImages, cultureInfo);
            }
        }
    }
}
