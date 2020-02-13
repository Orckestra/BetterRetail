using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Product.Tests.Factories
{
    [TestFixture]
    public class ProductViewModelFactoryGetProductDetailViewModelAsync : ProductViewModelFactoryTestBase
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(CreateDamProvider());
            _container.Use(CreateProductUrlProvider());
        }

        [Test]
        public void WHEN_GetProductParam_Is_Null_SHOULD_Throw_Argument_Exception()
        {
            //Arrange
            _container.Use(CreateDamProvider());
            _container.Use(CreateProductRepositoryWithNulls());
            _container.Use(CreateViewModelMapper());
            _container.Use(CreateProductViewModelFactory());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            Func<Task> asyncFunction = async () =>
            {
                await productViewModelFactory.GetProductViewModel(null);
            };

            //Assert
            asyncFunction.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_GetProductParam_Culture_Is_Null_SHOULD_Throw_Argument_Exception()
        {
            //Arrange
            _container.Use(CreateProductRepositoryWithNulls());
            _container.Use(CreateViewModelMapper());
            _container.Use(CreateProductViewModelFactory());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            Func<Task> asyncFunction = async () =>
            {
                await productViewModelFactory.GetProductViewModel(new GetProductParam
                {
                    Scope = GetRandom.String(32),
                    ProductId = GetRandom.String(32),
                    CultureInfo = null,
                    BaseUrl = GetRandom.String(32)
                });
            };

            //Assert
            asyncFunction.ShouldThrow<ArgumentException>();
        }

        [Test]
        public async Task WHEN_Product_Has_Lookup_Values_SHOULD_Return_Lookup_Localized_Values()
        {
            
            //Arrange
            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
                {
                    Scope = GetRandom.String(32),
                    ProductId = GetRandom.String(32),
                    CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                    BaseUrl = GetRandom.String(32),
                });

            //Assert
            model.Bag.Should().NotBeNull();
            model.Bag.Should().NotBeEmpty();
            model.Bag["Lookup1"].Should().Be("Look Up 1 FR");
            model.Bag["Size"].Should().Be("Medium|Small");
        }

        [Test]
        public async Task WHEN_min_quantity_is_less_than_1_SHOULD_quantity_is_null()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 0;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Should().BeNull();
        }

        [Test]
        public async Task WHEN_min_quantity_is_equal_to_1_SHOULD_quantity_is_not_null()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 1;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_min_quantity_is_greater_than_1_SHOULD_quantity_is_not_null()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 2;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_max_quantity_is_less_than_min_quantity_SHOULD_quantity_is_null()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 3;
            ProductConfiguration.MaxQuantity = 2;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Should().BeNull();
        }

        [Test]
        public async Task WHEN_max_quantity_is_equal_to_min_quantity_SHOULD_quantity_is_not_null()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 2;
            ProductConfiguration.MaxQuantity = 2;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_max_quantity_is_equal_to_min_quantity_SHOULD_quantity_value_is_min_quantity()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 2;
            ProductConfiguration.MaxQuantity = 2;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Min.Should().Be(ProductConfiguration.MinQuantity);
            model.Quantity.Max.Should().Be(ProductConfiguration.MaxQuantity);
            model.Quantity.Value.Should().Be(ProductConfiguration.MinQuantity);
        }

        [Test]
        public async Task WHEN_max_quantity_is_greater_than_min_quantity_SHOULD_quantity_is_not_null()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 1;
            ProductConfiguration.MaxQuantity = 2;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_max_quantity_is_greater_than_min_quantity_SHOULD_quantity_value_is_min_quantity()
        {
            //Arrange
            ProductConfiguration.MinQuantity = 1;
            ProductConfiguration.MaxQuantity = 2;

            _container.Use(CreateProductRepositoryWithLookups());
            _container.Use(CreateViewModelMapperTest());
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var model = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                Scope = GetRandom.String(32),
                ProductId = GetRandom.String(32),
                CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA"),
                BaseUrl = GetRandom.String(32)
            });

            //Assert
            model.Quantity.Min.Should().Be(ProductConfiguration.MinQuantity);
            model.Quantity.Max.Should().Be(ProductConfiguration.MaxQuantity);
            model.Quantity.Value.Should().Be(ProductConfiguration.MinQuantity);
        }

        private Mock<IProductUrlProvider> CreateProductUrlProvider()
        {
            Mock<IProductUrlProvider> productUrlProvider = new Mock<IProductUrlProvider>();

            productUrlProvider.Setup(p => p.GetProductUrl(It.IsAny<GetProductUrlParam>()))
                .Returns(() => GetRandom.String(128))
                .Verifiable();

            return productUrlProvider;
        }

        private static Mock<IDamProvider> CreateDamProvider()
        {
            Mock<IDamProvider> damProvider = new Mock<IDamProvider>(MockBehavior.Strict);

            List<ProductMainImage> dummyMainImages = new List<ProductMainImage>
            {
                new ProductMainImage
                {
                    ProductId = GetRandom.String(32),
                    VariantId = GetRandom.String(32),
                    ImageUrl  = GetRandom.WwwUrl(),
                }
            };

            List<AllProductImages> dummyAllProductImages = new List<AllProductImages>
            {
                new AllProductImages
                {
                    ProductId      = GetRandom.String(32),
                    VariantId      = GetRandom.String(32),
                    SequenceNumber = GetRandom.PositiveInt(),
                    ImageUrl       = GetRandom.WwwUrl(),
                    ThumbnailUrl   = GetRandom.WwwUrl(),
                }
            };

            damProvider.Setup(context => context.GetProductMainImagesAsync(It.IsNotNull<GetProductMainImagesParam>()))
                       .ReturnsAsync(dummyMainImages)
                       .Verifiable();

            damProvider.Setup(context => context.GetAllProductImagesAsync(It.IsNotNull<GetAllProductImagesParam>()))
                       .ReturnsAsync(dummyAllProductImages)
                       .Verifiable();

            return damProvider;
        }

        private static Mock<IProductRepository> CreateProductRepositoryWithNulls()
        {
            Mock<IProductRepository> productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);

            Overture.ServiceModel.Products.Product product = new Overture.ServiceModel.Products.Product();
            product.Variants = null; //A product without variants will eventually contain a null list
            product.Description = null; //A product without description will eventuall contain a null description

            product.Active = null;
            product.CatalogId = null;
            product.DefinitionName = null;
            product.Description = null;
            product.DisplayName = null;
            product.LastPublishedDate = null;
            product.ListPrice = null;
            product.NewProductDate = null;
            product.Prices = null;
            product.PrimaryParentCategoryId = null;
            product.Relationships = null;
            product.SequenceNumber = null;
            product.Sku = null;
            product.TaxCategory = null;
            product.Variants = null;


            productRepositoryMock.Setup(
                repo => repo.GetProductAsync(It.IsAny<GetProductParam>()))
                .ReturnsAsync(product)
                .Verifiable();

            return productRepositoryMock;
        }

        private static Mock<IProductRepository> CreateProductRepositoryWithLookups()
        {
            Mock<IProductRepository> productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag { { "Lookup1", "LookupValue1" }, { "Size", "Small|Medium" } }
            };

            productRepositoryMock.Setup(repo => repo.GetProductAsync(It.IsAny<GetProductParam>()))
                                 .ReturnsAsync(product)
                                 .Verifiable();

            productRepositoryMock.Setup(repo => repo.GetProductDefinitionAsync(It.IsAny<GetProductDefinitionParam>()))
                .ReturnsAsync(new ProductDefinition
                {
                    PropertyGroups = new List<ProductPropertyDefinitionGroup>
                    {
                        new ProductPropertyDefinitionGroup {
                            Properties = new List<ProductPropertyDefinition>
                            {
                                new ProductPropertyDefinition
                                {
                                    PropertyName = "Lookup1",
                                    DataType = PropertyDataType.Lookup,
                                    LookupDefinition = new ProductLookupDefinition
                                    {
                                        LookupName = "Lookup1Lookup"
                                    }
                                },
                                new ProductPropertyDefinition
                                {
                                    PropertyName = "Size",
                                    DataType = PropertyDataType.Lookup,
                                    LookupDefinition = new ProductLookupDefinition
                                    {
                                        LookupName = "SizeLookup"
                                    }
                                }
                            }
                        }
                    }
                })
                .Verifiable();

            return productRepositoryMock;
        }

        private static Mock<IViewModelMapper> CreateViewModelMapper()
        {
            Mock<IViewModelMapper> mapperMock = new Mock<IViewModelMapper>(MockBehavior.Strict);

            mapperMock.Setup(mapper => mapper.MapTo<ProductViewModel>(It.IsAny<Overture.ServiceModel.Products.Product>(), It.IsAny<CultureInfo>()))
                      .Returns(new ProductViewModel())
                      .Verifiable();

            return mapperMock;
        }

        private static Mock<IViewModelMapper> CreateViewModelMapperTest()
        {
            var mapperMock = new Mock<IViewModelMapper>(MockBehavior.Strict);

            var fakeProductDetailViewModel = new ProductViewModel();

            fakeProductDetailViewModel.GetType()
                .BaseType
                .GetProperty("Bag")
                .SetValue(fakeProductDetailViewModel, new Dictionary<string, object> { { "Lookup1", "LookupValue1" }, { "Size", "Medium|Small" } }, null);

            mapperMock
                .Setup(mapper => mapper.MapTo<ProductViewModel>(It.IsAny<Overture.ServiceModel.Products.Product>(), It.IsAny<CultureInfo>()))
                .Returns(fakeProductDetailViewModel)
                .Verifiable();

            return mapperMock;
        }

        private static Mock<IProductViewModelFactory> CreateProductViewModelFactory()
        {
            var productViewModelFactory = new Mock<IProductViewModelFactory>(MockBehavior.Strict);

            productViewModelFactory
                .Setup(p => p.GetProductViewModel(It.IsAny<GetProductParam>()))
                .ReturnsAsync(new ProductViewModel());

            return productViewModelFactory;
        }

    }
}
