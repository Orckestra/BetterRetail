using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Tests.Factories
{
    [TestFixture]
    public class ProductViewModelFactoryGetProductDetailViewKvaItemsAsync : ProductViewModelFactoryTestBase
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public async Task WHEN_Key_Variant_And_Culture_Exists_SHOULD_Return_Tree_List_With_Available_Values()
        {
            //Arrange
            var supportedCulture = CultureInfo.CreateSpecificCulture("fr-CA");
            _container.Use(CreateDamProvider());
            _container.Use(CreateProductRepository(supportedCulture, GenerateProductWithKva()));
            _container.Use(CreateViewModelMapper(supportedCulture));
            _container.Use(CreateLookupService());
            _container.Use(CreateProductSpecificationsViewService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var vm = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                ProductId = GetRandom.String(10),
                CultureInfo = supportedCulture,
                Scope = GetRandom.String(10),
                BaseUrl = GetRandom.String(32),
                
            })
            .ConfigureAwait(false);

            // Assert
            vm.KeyVariantAttributeItems.Should().NotBeNull();
            vm.KeyVariantAttributeItems.Count.Should().Be(4);

            vm.KeyVariantAttributeItems[0].DisplayName.Should().Be("Couleurs");
            vm.KeyVariantAttributeItems[0].PropertyName.Should().Be("Colour");

            //Text sorted alphabetically
            vm.KeyVariantAttributeItems[0].Values.Should().NotBeNull();
            vm.KeyVariantAttributeItems[0].Values.Count.Should().Be(2);
            vm.KeyVariantAttributeItems[0].Values[0].Value.Should().Be("Blue");
            vm.KeyVariantAttributeItems[0].Values[1].Value.Should().Be("Red");

            vm.KeyVariantAttributeItems[1].DisplayName.Should().Be("Tailles");
            vm.KeyVariantAttributeItems[1].PropertyName.Should().Be("Size");

            //Lookup sorted by lookup sort order
            vm.KeyVariantAttributeItems[1].Values.Should().NotBeNull();
            vm.KeyVariantAttributeItems[1].Values.Count.Should().Be(1);
            vm.KeyVariantAttributeItems[1].Values[0].Value.Should().Be("Small");

            vm.KeyVariantAttributeItems[2].DisplayName.Should().Be("Types");
            vm.KeyVariantAttributeItems[2].PropertyName.Should().Be("Type");

            vm.KeyVariantAttributeItems[2].Values.Should().NotBeNull();
            vm.KeyVariantAttributeItems[2].Values.Count.Should().Be(2);
            vm.KeyVariantAttributeItems[2].Values[0].Value.Should().Be("A");
            vm.KeyVariantAttributeItems[2].Values[1].Value.Should().Be("B");

            vm.KeyVariantAttributeItems[3].DisplayName.Should().Be("Look Up Title FR");
            vm.KeyVariantAttributeItems[3].PropertyName.Should().Be("Lookup1");

            vm.KeyVariantAttributeItems[3].Values.Should().NotBeNull();
            vm.KeyVariantAttributeItems[3].Values.Count.Should().Be(2);
            vm.KeyVariantAttributeItems[3].Values[0].Value.Should().Be("LookupValue3");
            vm.KeyVariantAttributeItems[3].Values[1].Value.Should().Be("LookupValue2");
        }

        [Test]
        public async Task WHEN_Key_Variant_Exists_And_Culture_Not_Supported_SHOULD_Return_Variants_With_Names_As_Keys()
        {
            //Arrange
            var notDefinedCulture = CultureInfo.CreateSpecificCulture("es-MX");
            _container.Use(CreateDamProvider());
            _container.Use(CreateProductRepository(notDefinedCulture, GenerateProductWithKva()));
            _container.Use(CreateViewModelMapper(notDefinedCulture));
            _container.Use(CreateLookupService());
            _container.Use(CreateProductSpecificationsViewService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var vm = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                ProductId = GetRandom.String(10), 
                CultureInfo = notDefinedCulture, 
                Scope = GetRandom.String(10),
                BaseUrl = GetRandom.String(32)
            })
            .ConfigureAwait(false);

            // Assert
            vm.KeyVariantAttributeItems.Should().NotBeNull();
            vm.KeyVariantAttributeItems.Count.Should().Be(4);

            vm.KeyVariantAttributeItems[0].DisplayName.Should().Be("Colour");
            vm.KeyVariantAttributeItems[1].DisplayName.Should().Be("Size");
            vm.KeyVariantAttributeItems[2].DisplayName.Should().Be("Type");
            vm.KeyVariantAttributeItems[3].DisplayName.Should().Be("Lookup1");
        }

        [Test]
        public async Task WHEN_Key_Variant_Is_Null_SHOULD_Return_KVA_List_Intanciated_With_Zero_Items()
        {
            //Arrange
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            _container.Use(CreateDamProvider());
            _container.Use(CreateProductRepository(culture, GenerateProductWithNullVariants()));
            _container.Use(CreateViewModelMapper(culture));
            _container.Use(CreateLookupService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var vm = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                ProductId = GetRandom.String(10), 
                CultureInfo = culture, 
                Scope = GetRandom.String(10),
                BaseUrl = GetRandom.String(32)
            })
            .ConfigureAwait(false);

            // Assert
            vm.KeyVariantAttributeItems.Should().NotBeNull();
            vm.KeyVariantAttributeItems.Count.Should().Be(0);
        }

        [Test]
        public async Task WHEN_Key_Variant_Order_Is_Defined_SHOULD_Return_KVA_List_With_Same_Order_Defined_For_KVA()
        {
            //Arrange
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            _container.Use(CreateDamProvider());
            _container.Use(CreateProductRepository(culture, GenerateProductWithKva()));
            _container.Use(CreateViewModelMapper(culture));
            _container.Use(CreateLookupService());
            _container.Use(CreateProductSpecificationsViewService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactory>();

            //Act
            var vm = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                ProductId = GetRandom.String(10), 
                CultureInfo = culture, 
                Scope = GetRandom.String(10),
                BaseUrl = GetRandom.String(32)
            })
            .ConfigureAwait(false);

            // Assert
            vm.KeyVariantAttributeItems.Should().NotBeNull();
            vm.KeyVariantAttributeItems.Count.Should().Be(4);

            vm.KeyVariantAttributeItems[0].DisplayName.Should().Be("Colors");
            vm.KeyVariantAttributeItems[1].DisplayName.Should().Be("Sizes");
            vm.KeyVariantAttributeItems[2].DisplayName.Should().Be("Types");
        }

        internal class ProductViewModelFactoryWithTestGetLookupImageUrl : ProductViewModelFactory
        {
            public ProductViewModelFactoryWithTestGetLookupImageUrl(
                IViewModelMapper viewModelMapper, 
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

            protected override string GetLookupImageUrl(string lookupName, object value)
            {
                if (lookupName == "Lookup1Lookup" && "LookupValue3".Equals(value))
                {
                    return "www.some.url/for/lookup3.png";
                }

                return base.GetLookupImageUrl(lookupName, value);
            }
        }
        /// <summary>
        /// Tests an extension point used by some projects
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task WHEN_KVAValue_is_known_by_lookup_SHOULD_have_kva_imageUrl_as_returned_by_GetLookupImageUrl()
        {
            //Arrange
            var supportedCulture = CultureInfo.CreateSpecificCulture("fr-CA");
            _container.Use(CreateDamProvider());
            _container.Use(CreateProductRepository(supportedCulture, GenerateProductWithKva()));
            _container.Use(CreateViewModelMapper(supportedCulture));
            _container.Use(CreateLookupService());
            _container.Use(CreateProductSpecificationsViewService());

            var productViewModelFactory = _container.CreateInstance<ProductViewModelFactoryWithTestGetLookupImageUrl>();

            //Act
            var vm = await productViewModelFactory.GetProductViewModel(new GetProductParam
            {
                ProductId = GetRandom.String(10),
                CultureInfo = supportedCulture,
                Scope = GetRandom.String(10),
                BaseUrl = GetRandom.String(32)
            })
            .ConfigureAwait(false);

            // Assert
            vm.KeyVariantAttributeItems
                .First(kva => kva.PropertyName == "Lookup1")
                .Values.First(value => "LookupValue3".Equals(value.Value))
                .ImageUrl.Should().Be("www.some.url/for/lookup3.png");

            vm.KeyVariantAttributeItems
                .First(kva => kva.PropertyName == "Lookup1")
                .Values.First(value => "LookupValue2".Equals(value.Value))
                .ImageUrl.Should().BeNullOrEmpty("Because no image is resolved for this lookup value");
        }

        #region Mock
        private static Mock<IDamProvider> CreateDamProvider()
        {
            Mock<IDamProvider> damProvider = new Mock<IDamProvider>();

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
                    ProductId           = GetRandom.String(32),
                    VariantId           = GetRandom.String(32),
                    SequenceNumber      = GetRandom.PositiveInt(),
                    ImageUrl            = GetRandom.WwwUrl(),
                    ThumbnailUrl        = GetRandom.WwwUrl(),
                    ProductZoomImageUrl = GetRandom.WwwUrl(),
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

        private static Mock<IProductRepository> CreateProductRepository(CultureInfo culture, Overture.ServiceModel.Products.Product product)
        {
            Mock<IProductRepository> productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(
                repo => repo.GetProductAsync(It.Is<GetProductParam>(param => param.CultureInfo.Name == culture.Name)))
                .ReturnsAsync(product)
                .Verifiable();

            productRepositoryMock.Setup(
                repo => repo.GetProductDefinitionAsync(It.Is<GetProductDefinitionParam>(param => param.Name == null && param.CultureInfo.Name == culture.Name)))
                .ReturnsAsync(GenerateProductDefinitionWithKva())
                .Verifiable();

            return productRepositoryMock;
        }

        private static Mock<IViewModelMapper> CreateViewModelMapper(CultureInfo culture)
        {
            Mock<IViewModelMapper> mapperMock = new Mock<IViewModelMapper>();
            
            mapperMock.Setup(mapper => mapper.MapTo<ProductViewModel>(It.Is<Overture.ServiceModel.Products.Product>(r => r != null), It.Is<CultureInfo>(c => c.Name == culture.Name)))
                      .Returns(new ProductViewModel())
                      .Verifiable();

            mapperMock.Setup(mapper => mapper.MapTo<VariantViewModel>(It.IsNotNull<Variant>(), It.Is<CultureInfo>(c => c.Name == culture.Name)))
                      .Returns(new VariantViewModel())
                      .Verifiable();

            return mapperMock;
        }

        private static Mock<IProductSpecificationsViewService> CreateProductSpecificationsViewService()
        {
            var specificationMock = new SpecificationsViewModel
            {
                ProductId = GetRandom.String(32)
            };

            Mock<IProductSpecificationsViewService> productSpecificationsViewServiceMock = new Mock<IProductSpecificationsViewService>();
            productSpecificationsViewServiceMock.Setup(
                service => service.GetProductSpecificationsViewModel(It.IsNotNull<GetProductSpecificationsParam>()))
                .Returns(specificationMock)
                .Verifiable();

            return productSpecificationsViewServiceMock;
        }
        #endregion

        #region Fake data for products/products defintion.

        private static Overture.ServiceModel.Products.Product GenerateProductWithKva()
        {

            /*
            
            Product Variant Values.
            _____________________________________
            |    Color       Size    Type       |
            |___________________________________|
            |    Red         Small       A      |
            |    Blue        Small       B      |
            |    White       Medium      C      |
            |___________________________________|

            
            Assuming that thoses are KVA, they should be transformed into 3 lists:

            1: Red, Blue, White.
            2: Smal, Medium.
            3: A, B, C.

            */

            var variantOne = new Variant();
            variantOne.Id = "VariantOne";
            variantOne.Active = true;
            variantOne.PropertyBag = new PropertyBag();
            variantOne.PropertyBag.Add("Size", "Small");
            variantOne.PropertyBag.Add("Colour", "Red");
            variantOne.PropertyBag.Add("Type", "A");
            variantOne.PropertyBag.Add("Lookup1", "LookupValue3");

            var variantTwo = new Variant();
            variantTwo.Id = "VariantTwo";
            variantTwo.Active = true;
            variantTwo.PropertyBag = new PropertyBag();
            variantTwo.PropertyBag.Add("Size", "Small");
            variantTwo.PropertyBag.Add("Colour", "Blue");
            variantTwo.PropertyBag.Add("Type", "B");
            variantTwo.PropertyBag.Add("Lookup1", "LookupValue2");

            var variantThree = new Variant();
            variantThree.Id = "variantThree";
            variantThree.Active = false;
            variantThree.PropertyBag = new PropertyBag();
            variantThree.PropertyBag.Add("Size", "Medium");
            variantThree.PropertyBag.Add("Colour", "White");
            variantThree.PropertyBag.Add("Type", "C");
            variantThree.PropertyBag.Add("Lookup1", "LookupValue1");

            var product = new Overture.ServiceModel.Products.Product();
            product.Variants = new List<Variant>();
            product.Variants.Add(variantOne);
            product.Variants.Add(variantTwo);
            product.Variants.Add(variantThree);

            return product;
        }

        private static Overture.ServiceModel.Products.Product GenerateProductWithNullVariants()
        {
            return new Overture.ServiceModel.Products.Product();
        }

        private static ProductDefinition GenerateProductDefinitionWithKva()
        {
            var productDefinition = new ProductDefinition();
            productDefinition.VariantProperties = new List<ProductPropertyDefinition>();

            var color = new ProductPropertyDefinition();
            color.PropertyName = "Colour";
            color.DisplayName = new LocalizedString();
            color.DisplayName.Add("en-US", "Colors");
            color.DisplayName.Add("fr-CA", "Couleurs");
            color.IsRequired = true;
            color.DisplayOrder = 0;
            color.DataType = PropertyDataType.Text;
            color.DefaultValue = "Black";
            color.IsKeyVariant = true;
            color.KeyVariantOrder = 0;

            var size = new ProductPropertyDefinition();
            size.PropertyName = "Size";
            size.DisplayName = new LocalizedString();
            size.DisplayName.Add("en-US", "Sizes");
            size.DisplayName.Add("fr-CA", "Tailles");
            size.IsRequired = true;
            // size.DisplayOrder = 1;
            size.DisplayOrder = 0;
            size.DataType = PropertyDataType.Lookup;
            size.DefaultValue = "Medium";
            size.IsKeyVariant = true;
            size.KeyVariantOrder = 1;
            size.LookupDefinition = new ProductLookupDefinition
            {
                LookupName = "SizeLookup"
            };

            var type = new ProductPropertyDefinition();
            type.PropertyName = "Type";
            type.DisplayName = new LocalizedString();
            type.DisplayName.Add("en-US", "Types");
            type.DisplayName.Add("fr-CA", "Types");
            type.IsRequired = true;
            // type.DisplayOrder = 2;
            type.DisplayOrder = 0;
            type.DataType = PropertyDataType.Text;
            type.DefaultValue = "A";
            type.IsKeyVariant = true;
            type.KeyVariantOrder= 2;

            var lookup = new ProductPropertyDefinition();
            lookup.PropertyName = "Lookup1";
            lookup.DisplayName = new LocalizedString();
            lookup.DisplayName.Add("en-US", "Look Up Title EN");
            lookup.DisplayName.Add("fr-CA", "Look Up Title FR");
            lookup.IsRequired = true;
            //lookup.DisplayOrder = 3;
            lookup.DisplayOrder = 0;
            lookup.DataType = PropertyDataType.Lookup;
            lookup.DefaultValue = null;
            lookup.IsKeyVariant = true;
            lookup.KeyVariantOrder = 3;
            lookup.LookupDefinition = new ProductLookupDefinition
            {
                LookupName = "Lookup1Lookup"
            };

            productDefinition.VariantProperties.Add(size);  // Display Order : 1
            productDefinition.VariantProperties.Add(color); // Display Order : 0
            productDefinition.VariantProperties.Add(type);  // Display Order : 2
            productDefinition.VariantProperties.Add(lookup);// Display Order : 3

            return productDefinition;
        }


        #endregion
    }
}
