using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;
using Orckestra.Overture.ServiceModel.Products;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Tests.Providers.Dam
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class ConventionBaseDamProvider_GetAllProductImagesAsync
    {
        private AutoMocker _container;
        private Mock<ISiteConfiguration> _siteConfigurationMock;
        private Mock<ICdnDamProviderSettings> _cdmproviderMock;
        private MediaSettings _mediaSettings;
        private Mock<IProductMediaSettingsRepository> _productMediaSettingsRepositoryMock;


        [SetUp]
        public void SetUp()
        {
            _cdmproviderMock = new Mock<ICdnDamProviderSettings>();
            _cdmproviderMock.Setup(c => c.ProductImageFilePathPattern).Returns("{productId}_{sequenceNumber}_{imageSize}.jpg");
            _cdmproviderMock.Setup(c => c.VariantImageFilePathPattern).Returns("{productId}_{variantId}_{sequenceNumber}_{imageSize}.jpg");
            _cdmproviderMock.Setup(c => c.SupportXLImages).Returns(true);

            _siteConfigurationMock = new Mock<ISiteConfiguration>();
            _siteConfigurationMock.Setup(s => s.CdnDamProviderSettings).Returns(_cdmproviderMock.Object);

            _mediaSettings = new MediaSettings()
            {
                MediaServerUrl = "https://occdev0localdeployment.blob.core.windows.net/s0011dv17-media/",
                MediaFallbackImageName = "image_not_found.jpg"
            };

            _productMediaSettingsRepositoryMock = new Mock<IProductMediaSettingsRepository>();
            _productMediaSettingsRepositoryMock.Setup(ms => ms.GetProductMediaSettings()).ReturnsAsync(_mediaSettings);

            _container = new AutoMocker();
            _container.Use(_siteConfigurationMock);
        }

        [Test]
        public void WHEN_Passing_Null_Parameter_SHOULD_Throw_Exception()
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            // Act & Assert
            Expression<Func<Task<List<AllProductImages>>>> expression = () => damProvider.GetAllProductImagesAsync(null);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ImageSize_Within_Parameter_SHOULD_Throw_Exception(string imageSize)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();
            var param = new GetAllProductImagesParam()
            {
                ImageSize = imageSize,
                ThumbnailImageSize = GetRandom.String(1),
                ProductZoomImageSize = GetRandom.String(1),
                ProductId = GetRandom.String(10)
            };

            // Act & Assert
            Expression<Func<Task<List<AllProductImages>>>> expression = () => damProvider.GetAllProductImagesAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ImageSize)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ThumbnailImageSize_Within_Parameter_SHOULD_Throw_Exception(string thumbnailImageSize)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();
            var param = new GetAllProductImagesParam()
            {
                ImageSize = GetRandom.String(1),
                ThumbnailImageSize = thumbnailImageSize,
                ProductZoomImageSize = GetRandom.String(1),
                ProductId = GetRandom.String(10)
            };

            // Act & Assert
            Expression<Func<Task<List<AllProductImages>>>> expression = () => damProvider.GetAllProductImagesAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ThumbnailImageSize)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ProductZoomImageSize_Within_Parameter_SHOULD_Throw_Exception(string productZoomImageSize)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();
            var param = new GetAllProductImagesParam()
            {
                ImageSize = GetRandom.String(1),
                ThumbnailImageSize = GetRandom.String(1),
                ProductZoomImageSize = productZoomImageSize,
                ProductId = GetRandom.String(10)
            };

            // Act & Assert
            Expression<Func<Task<List<AllProductImages>>>> expression = () => damProvider.GetAllProductImagesAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ProductZoomImageSize)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Passing_Null_Or_Empty_ProductId_Within_Parameter_SHOULD_Throw_Exception(string productId)
        {
            // Arrange
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();
            var param = new GetAllProductImagesParam()
            {
                ImageSize = GetRandom.String(1),
                ThumbnailImageSize = GetRandom.String(1),
                ProductZoomImageSize = GetRandom.String(1),
                ProductId = productId
            };

            // Act & Assert
            Expression<Func<Task<List<AllProductImages>>>> expression = () => damProvider.GetAllProductImagesAsync(param);
            var exception = Assert.ThrowsAsync<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.ProductId)));
        }

        [Test]
        public async Task WHEN_Passing_Valid_Values_With_No_Variants_Within_Parameter_SHOULD_Succeed()
        {
            // Arrange
            var expectedProductId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();
            var param = new GetAllProductImagesParam()
            {
                ImageSize = GetRandom.String(1),
                ThumbnailImageSize = GetRandom.String(1),
                ProductZoomImageSize = GetRandom.String(1),
                ProductId = expectedProductId,
                Variants = null
            };

            // Act
            var results = await damProvider.GetAllProductImagesAsync(param).ConfigureAwait(false);

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
                Variants = new List<Variant>()
                {
                    new Variant
                    {
                        Id = variantId1,
                    },
                    new Variant
                    {
                        Id = variantId2,
                    },
                    new Variant
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

        private ProductMedia GetProductMediaMock(string[] sizeInstances)
        {
            var ResizedInstances = sizeInstances.Select(size => new ResizedMediaLink()
            {
                Size = size,
                Url = "~/" + GetRandom.String(32) + ".jpg",
            });

            return new ProductMedia()
            {
                MediaType = nameof(MediaTypeEnum.Image),
                Url = "~/" + GetRandom.String(32) + ".jpg",
                ResizedInstances = ResizedInstances.ToArray()
            };
        }

        [Test]
        public async Task WHEN_Passing_Valid_Values_With_MediaSet_Within_Parameter_SHOULD_Succeed()
        {
            _container.Use(_productMediaSettingsRepositoryMock);

            // Arrange
            var expectedProductId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            var ImageSize = GetRandom.String(1);
            var ThumbnailImageSize = GetRandom.String(1);
            var ProductZoomImageSize = GetRandom.String(1);
            var param = new GetAllProductImagesParam()
            {
                ImageSize = ImageSize,
                ThumbnailImageSize = ThumbnailImageSize,
                ProductZoomImageSize = ProductZoomImageSize,
                ProductId = expectedProductId,
                Variants = null,
                MediaSet = new List<ProductMedia>()
                {
                    GetProductMediaMock(new string[] { ImageSize, ThumbnailImageSize, ProductZoomImageSize })
                }
            };

            // Act
            var results = await damProvider.GetAllProductImagesAsync(param).ConfigureAwait(false);

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
        public async Task WHEN_Passing_Valid_Values_With_MediaSet_Variants_Within_Parameter_SHOULD_Succeed()
        {
            _container.Use(_productMediaSettingsRepositoryMock);

            // Arrange
            var expectedProductId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            var ImageSize = GetRandom.String(1);
            var ThumbnailImageSize = GetRandom.String(1);
            var ProductZoomImageSize = GetRandom.String(1);
            var PropertyBag = new Overture.ServiceModel.PropertyBag()
            {
                { GetRandom.String(5), GetRandom.String(5) as object }
            };

            var param = new GetAllProductImagesParam()
            {
                ImageSize = ImageSize,
                ThumbnailImageSize = ThumbnailImageSize,
                ProductZoomImageSize = ProductZoomImageSize,
                ProductId = expectedProductId,
                Variants = new List<Variant>()
                {
                    new Variant
                    {
                        Id = GetRandom.String(5),
                        PropertyBag = PropertyBag
                    },
                    new Variant
                    {
                        Id = GetRandom.String(5),
                        MediaSet = new List<ProductMedia>()
                        {
                            GetProductMediaMock(new string[] { ImageSize, ThumbnailImageSize, ProductZoomImageSize }),
                        }
                    },
                },
                MediaSet = new List<ProductMedia>()
                {
                    GetProductMediaMock(new string[] { ImageSize, ThumbnailImageSize, ProductZoomImageSize })
                },
                VariantMediaSet = new List<VariantMediaSet>()
                {
                    new VariantMediaSet()
                    {
                        Media = new ProductMedia[]
                        {
                             GetProductMediaMock(new string[] { ImageSize, ThumbnailImageSize, ProductZoomImageSize }),
                        },
                        AttributesToMatch = PropertyBag
                    }
                }
            };

            // Act
            var results = await damProvider.GetAllProductImagesAsync(param).ConfigureAwait(false);

            // Assert
            results.Should().NotBeEmpty();
            results.Where(result => string.IsNullOrWhiteSpace(result.ImageUrl)).Should().BeEmpty("Because all results should have a known default Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.FallbackImageUrl)).Should().BeEmpty("Because all results should have a known fallback Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ThumbnailUrl)).Should().BeEmpty("Because all results should have a known thumbnail Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ProductZoomImageUrl)).Should().BeEmpty("Because all results should have a known Product Zoom Image url");
            results.Where(result => result.ProductId != expectedProductId).Should().BeEmpty("Because all results should match the requested product id");
            results.Where(result => result.ProductId == expectedProductId).Should().NotBeNullOrEmpty("Because the product must be found");
            results.Where(result => results.Where(x => x.ImageUrl == result.ImageUrl).ToList().Count > 1).Should().BeEmpty("Because all results should have unique ImageUrl");
        }

        [Test]
        public async Task WHEN_Passing_Valid_Values_With_One_MediaSet_Variants_Within_Parameter_SHOULD_Succeed()
        {
            _container.Use(_productMediaSettingsRepositoryMock);

            // Arrange
            var expectedProductId = GetRandom.String(32);
            IDamProvider damProvider = _container.CreateInstance<ConventionBasedDamProvider>();

            var ImageSize = GetRandom.String(1);
            var ThumbnailImageSize = GetRandom.String(1);
            var ProductZoomImageSize = GetRandom.String(1);
            var PropertyBag = new Overture.ServiceModel.PropertyBag()
            {
                { GetRandom.String(5), GetRandom.String(5) as object }
            };
            var productMedia = GetProductMediaMock(new string[] { ImageSize, ThumbnailImageSize, ProductZoomImageSize });
            var variant1Id = GetRandom.String(5);
            var variant2Id = GetRandom.String(5);
            var imageUrl = productMedia.ResizedInstances.FirstOrDefault(instance => instance.Size == ImageSize).Url.Substring(1);

            var param = new GetAllProductImagesParam()
            {
                ImageSize = ImageSize,
                ThumbnailImageSize = ThumbnailImageSize,
                ProductZoomImageSize = ProductZoomImageSize,
                ProductId = expectedProductId,
                Variants = new List<Variant>()
                {
                    new Variant
                    {
                        Id = variant1Id,
                        PropertyBag = PropertyBag
                    },
                    new Variant
                    {
                        Id = variant2Id,
                        MediaSet = new List<ProductMedia>()
                        {
                            productMedia,
                        }
                    },
                },
            };

            // Act
            var results = await damProvider.GetAllProductImagesAsync(param).ConfigureAwait(false);

            // Assert
            results.Should().NotBeEmpty();
            results.Where(result => string.IsNullOrWhiteSpace(result.ImageUrl)).Should().NotBeNullOrEmpty("Because results should have a unknown default Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.FallbackImageUrl)).Should().BeEmpty("Because all results should have a known fallback Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ThumbnailUrl)).Should().NotBeNullOrEmpty("Because results should have a unknown thumbnail Image url");
            results.Where(result => string.IsNullOrWhiteSpace(result.ProductZoomImageUrl)).Should().NotBeNullOrEmpty("Because results should have a unknown Product Zoom Image url");
            results.Where(result => result.ProductId != expectedProductId).Should().BeEmpty("Because all results should match the requested product id");
            results.Where(result => result.ProductId == expectedProductId).Should().NotBeNullOrEmpty("Because the product must be found");

            results.Find(result => result.VariantId == null).ImageUrl.Should().Be("");
            results.Find(result => result.VariantId == variant1Id).ImageUrl.Should().Be("");
            results.Find(result => result.VariantId == variant2Id).ImageUrl.Contains(imageUrl).Should().Be(true);
        }
    }
}
