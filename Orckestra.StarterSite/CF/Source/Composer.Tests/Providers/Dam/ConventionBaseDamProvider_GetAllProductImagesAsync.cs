using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;

namespace Orckestra.Composer.Tests.Providers.Dam
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class ConventionBaseDamProvider_GetAllProductImagesAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [TearDown]
        public void TearDown()
        {
            _container.VerifyAll();
        }

        [Test]
        public void WHEN_Passing_Null_Parameter_SHOULD_Throw_Exception()
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                damProvider.GetAllProductImagesAsync(null);
            });

            exception.Message.Should().Be("The method parameter is required.\r\nParameter name: param");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ImageSize_Within_Parameter_SHOULD_Throw_Exception(string imageSize)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                damProvider.GetAllProductImagesAsync(new GetAllProductImagesParam()
                {
                    ImageSize = imageSize,
                    ThumbnailImageSize = GetRandom.String(1),
                    ProductZoomImageSize = GetRandom.String(1),
                    ProductId = GetRandom.String(10)
                });
            });

            exception.Message.Should().Be("The image size is required.");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ThumbnailImageSize_Within_Parameter_SHOULD_Throw_Exception(string thumbnailImageSize)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                damProvider.GetAllProductImagesAsync(new GetAllProductImagesParam()
                {
                    ImageSize = GetRandom.String(1),
                    ThumbnailImageSize = thumbnailImageSize,
                    ProductZoomImageSize = GetRandom.String(1),
                    ProductId = GetRandom.String(10)
                });
            });

            exception.Message.Should().Be("The thumbnail image size is required.");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ProductZoomImageSize_Within_Parameter_SHOULD_Throw_Exception(string productZoomImageSize)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                damProvider.GetAllProductImagesAsync(new GetAllProductImagesParam()
                {
                    ImageSize = GetRandom.String(1),
                    ThumbnailImageSize = GetRandom.String(1),
                    ProductZoomImageSize = productZoomImageSize,
                    ProductId = GetRandom.String(10)
                });
            });

            exception.Message.Should().Be("The product zoom image size is required.");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ProductId_Within_Parameter_SHOULD_Throw_Exception(string productId)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                damProvider.GetAllProductImagesAsync(new GetAllProductImagesParam()
                {
                    ImageSize = GetRandom.String(1),
                    ThumbnailImageSize = GetRandom.String(1),
                    ProductZoomImageSize = GetRandom.String(1),
                    ProductId = productId
                });
            });

            exception.Message.Should().Be("The product id is required.");
        }

        [Test]
        public async Task WHEN_Passing_Valid_Values_With_No_Variants_Within_Parameter_SHOULD_Succeed()
        {
            // Arrange
            var expectedProductId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act
            var results = await damProvider.GetAllProductImagesAsync(new GetAllProductImagesParam()
            {
                ImageSize = GetRandom.String(1),
                ThumbnailImageSize = GetRandom.String(1),
                ProductZoomImageSize = GetRandom.String(1),
                ProductId = expectedProductId,
                Variants = null
            }).ConfigureAwait(false);

            // Assert
            results.Should().NotBeEmpty();
            results.ForEach(x => x.VariantId.Should().BeNull("There should be no variants because it was not specified in the request."));
            results.Where(result => string.IsNullOrWhiteSpace(result.ImageUrl)).Should().BeEmpty("Because all results should have a known default Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.FallbackImageUrl)).Should().BeEmpty("Because all results should have a known fallback Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ThumbnailUrl)).Should().BeEmpty("Because all results should have a known thumbnail Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ProductZoomImageUrl)).Should().BeEmpty("Because all results should have a known Product Zoom Image url");
            results.Where(result => result.ProductId != expectedProductId).Should().BeEmpty("Because all results should match the requested product id");
            results.Where(result => result.ProductId == expectedProductId).Should().NotBeNullOrEmpty("Because the product must be found");
        }

        [Test]
        public async Task WHEN_Passing_Valid_Values_With_Variants_Within_Parameter_SHOULD_Succeed()
        {
            // Arrange
            var expectedProductId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();
            var variantId1 = GetRandom.String(5);
            var variantId2 = GetRandom.String(5);
            var variantId3 = GetRandom.String(5);

            // Act
            var results = await damProvider.GetAllProductImagesAsync(new GetAllProductImagesParam()
            {
                ImageSize = GetRandom.String(1),
                ThumbnailImageSize = GetRandom.String(1),
                ProductZoomImageSize = GetRandom.String(1),
                ProductId = expectedProductId,
                Variants = new List<VariantKey>()
                {
                    new VariantKey
                    {
                        Id = variantId1,
                    },
                    new VariantKey
                    {
                        Id = variantId2,
                    },
                    new VariantKey
                    {
                        Id = variantId3,
                    }
                }
            }).ConfigureAwait(false);

            // Assert
            results.Should().NotBeNull();
            int index = 0;
            results.ForEach(x =>
            {
                
                if (index % ConventionBasedDamProviderConfiguration.MaxThumbnailImages == 0)
                {
                    x.VariantId.Should().BeNull("This is a main image for a product only.");
                }
                else if (index % ConventionBasedDamProviderConfiguration.MaxThumbnailImages == 1)
                {
                    x.VariantId.Should().Be(variantId1);
                }
                else if (index % ConventionBasedDamProviderConfiguration.MaxThumbnailImages == 2)
                {
                    x.VariantId.Should().Be(variantId2);
                }
                else if (index % ConventionBasedDamProviderConfiguration.MaxThumbnailImages == 3)
                {
                    x.VariantId.Should().Be(variantId3);
                }

                index++;
            });

            results.Should().NotBeEmpty();
            results.Where(result => string.IsNullOrWhiteSpace(result.ImageUrl)).Should().BeEmpty("Because all results should have a known default Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.FallbackImageUrl)).Should().BeEmpty("Because all results should have a known fallback Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ThumbnailUrl)).Should().BeEmpty("Because all results should have a known thumbnail Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ProductZoomImageUrl)).Should().BeEmpty("Because all results should have a known Product Zoom Image url");
            results.Where(result => result.ProductId != expectedProductId).Should().BeEmpty("Because all results should match the requested product id");
            results.Where(result => result.ProductId == expectedProductId).Should().NotBeNullOrEmpty("Because the product must be found");
        }

    }
}
