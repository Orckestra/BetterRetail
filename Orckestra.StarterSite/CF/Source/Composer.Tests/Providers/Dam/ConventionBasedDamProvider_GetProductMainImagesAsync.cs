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
    public class ConventionBasedDamProvider_GetProductMainImagesAsync
    {
        private AutoMocker _container;
        //

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
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            var expectedProductId = GetRandom.String(32);
            var expectedVariantId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act
            List<ProductMainImage> images = await damProvider.GetProductMainImagesAsync(new GetProductMainImagesParam
            {
                ImageSize = GetRandom.String(1),
                ProductImageRequests = new ProductImageRequest[] 
                {
                    new ProductImageRequest 
                    {
                        ProductId = expectedProductId,
                        Variant = new VariantKey
                        {
                            Id = expectedVariantId
                        }
                    }
                }.ToList(),
            }).ConfigureAwait(false);

            // Assert
            images.Should().NotBeNull("It is ok to return an empty list, but not ok to return null");
            images.Where(result => string.IsNullOrWhiteSpace(result.ImageUrl)).Should().BeEmpty("Because all results should have a known default Image url");
            images.Where(result => string.IsNullOrWhiteSpace(result.FallbackImageUrl)).Should().BeEmpty("Because all results should have a known fallback Image url");
            images.Where(result => result.ProductId != expectedProductId).Should().BeEmpty("Because all results should match the requested product id");
            images.Where(result => result.ProductId == expectedProductId).Should().NotBeNullOrEmpty("Because the product must be found");
            images.Where(result => result.ProductId == expectedProductId && result.VariantId == expectedVariantId).Should().NotBeNullOrEmpty("Because the variant must be found");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public async Task WHEN_VariantId_Is_NullOrWhiteSpace_SHOULD_Succeed(string variantId)
        {
            //Arrange
            var expectedProductId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act
            List<ProductMainImage> images = await damProvider.GetProductMainImagesAsync(new GetProductMainImagesParam
            {
                ImageSize = GetRandom.String(1),
                ProductImageRequests = new ProductImageRequest[] 
                {
                    new ProductImageRequest 
                    {
                        ProductId = expectedProductId,
                        Variant = new VariantKey
                        {
                            Id = GetRandom.String(32)
                        }
                    }
                }.ToList(),
            }).ConfigureAwait(false);

            // Assert
            images.Should().NotBeNull("Empty variantID actually means we want the images for the product");
            images.Where(result => string.IsNullOrWhiteSpace(result.ImageUrl)).Should().BeEmpty("Because all results should have a known default Image url");
            images.Where(result => string.IsNullOrWhiteSpace(result.FallbackImageUrl)).Should().BeEmpty("Because all results should have a known fallback Image url");
            images.Where(result => result.ProductId != expectedProductId).Should().BeEmpty("Because all results should match the requested product id");
            images.Where(result => result.ProductId == expectedProductId).Should().NotBeNullOrEmpty("Because the product must be found");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_ProductDefinion_SHOULD_Throw_ArgumentException(string productId)
        {
            //Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                damProvider.GetProductMainImagesAsync(new GetProductMainImagesParam
                {
                    ImageSize = GetRandom.String(1),
                    ProductImageRequests = new ProductImageRequest[]
                    {
                        new ProductImageRequest
                        {
                            ProductId = productId,
                            Variant = new VariantKey
                            {
                                Id = GetRandom.String(32)
                            }
                        }
                    }.ToList(),
                });
            });
        }

        [Test]
        public async Task WHEN_KeyVariantAttributeValues_Is_Null_SHOULD_Succeed()
        {
            //Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act
            List<ProductMainImage> images = await damProvider.GetProductMainImagesAsync(new GetProductMainImagesParam
            {
                ImageSize = GetRandom.String(1),
                ProductImageRequests = new ProductImageRequest[] 
                {
                    new ProductImageRequest 
                    {
                        ProductId = GetRandom.String(32),
                        Variant = new VariantKey
                        {
                            Id = GetRandom.String(32)
                        }
                    }
                }.ToList(),
            }).ConfigureAwait(false);

            // Assert
            images.Should().NotBeNull();
        }

        [Test]
        public void WHEN_ProductImageInfos_Is_Null_SHOULD_Throw_ArgumentException()
        {
            //Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                damProvider.GetProductMainImagesAsync(new GetProductMainImagesParam
                {
                    ImageSize = GetRandom.String(1),
                    ProductImageRequests = null,
                });
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_ImageSize_Is_NullOrWhiteSpace_SHOULD_Throw_ArgumentException(string imageSize)
        {
            //Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                damProvider.GetProductMainImagesAsync(new GetProductMainImagesParam
                {
                    ImageSize = imageSize,
                    ProductImageRequests = new ProductImageRequest[] 
                    {
                        new ProductImageRequest 
                        {
                            ProductId = GetRandom.String(32),
                            Variant = new VariantKey
                            {
                                Id = GetRandom.String(32)
                            }
                        }
                    }.ToList(),
                });
            });
        }

        [Test]
        public void WHEN_Passing_Null_Parameters_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                damProvider.GetProductMainImagesAsync(null);
            });
        }
    }
}
