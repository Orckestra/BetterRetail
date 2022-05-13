using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Exceptions;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class ProductSpecificationsViewServiceGetProductSpecificationsViewModelAsync
    {
        private const string ProductId = "Product1";
        private const string VariantId = "Variant1";

        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            SetupComposerContext();
            SetupLocalizationProvider();
            SetupLookupService();
        }

        private void SetupComposerContext()
        {
            var composerContext = new Mock<IComposerContext>();
            composerContext.Setup(context => context.CultureInfo).Returns(CultureInfo.GetCultureInfo("en-US"));

            _container.Use(composerContext);
        }

        private void SetupLocalizationProvider()
        {
            var localizationProviderMock = new Mock<ILocalizationProvider>();

            localizationProviderMock.Setup(l => l.GetLocalizedString(It.IsAny<GetLocalizedParam>())).Returns("{0}");
            localizationProviderMock.Setup(l => l.GetLocalizedString(It.Is<GetLocalizedParam>(p => p.Key == "attrWithCustomResource"))).Returns("{0} customText");

            _container.Use(localizationProviderMock);
        }

        private void SetupLookupService()
        {
            var lookupService = new Mock<ILookupService>();

            lookupService.Setup(l => l.GetLookupDisplayNameAsync(It.Is<GetLookupDisplayNameParam>(p => p.Value == "test1|test2"))).ReturnsAsync("test 1, test 2");
            lookupService.Setup(l => l.GetLookupDisplayNameAsync(It.Is<GetLookupDisplayNameParam>(p => p.Value == "test1"))).ReturnsAsync("test 1");

            _container.Use(lookupService);
        }

        [Test]
        public void WHEN_null_parameter_SHOULD_throw_ArgumentException()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.Throws<ArgumentNullException>(() => productSpecificationsViewService.GetProductSpecificationsViewModel(null));
        }

        [Test]
        public void WHEN_product_is_null_SHOULD_throw_ArgumentException()
        {
            // Arrange
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.Throws<ArgumentException>(() => productSpecificationsViewService.GetProductSpecificationsViewModel(
                new GetProductSpecificationsParam
                {
                    Product = null
                }
            ));
        }

        [Test]
        public void WHEN_variant_id_is_null_SHOULD_not_throw_ArgumentException()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag(),
                Variants = new List<Variant>()
            };

            var productDefinition = BuildProductDefinitionWithEachAttributeTypes();
            SetupProductRepository(product, productDefinition);

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.DoesNotThrow(() => productSpecificationsViewService.GetProductSpecificationsViewModel(
                new GetProductSpecificationsParam
                {
                    Product = product,
                    ProductDefinition = productDefinition,
                    VariantId = null
                }
            ));
        }

        [Test]
        public void WHEN_variant_id_is_empty_SHOULD_not_throw_ArgumentException()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag(),
                Variants = new List<Variant>()
            };

            var productDefinition = BuildProductDefinitionWithEachAttributeTypes();

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Assert
            Assert.DoesNotThrow(() => productSpecificationsViewService.GetProductSpecificationsViewModel(
                new GetProductSpecificationsParam
                {
                    Product = product,
                    ProductDefinition = productDefinition,
                    VariantId = string.Empty
                }
            ));
        }

        [Test]
        public void WHEN_variant_found_SHOULD_return_variant_specifications()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag
                {
                    {"attrText", "test1"}
                },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag
                        {
                            {"attrText", "test2"}
                        }
                    }
                }
            };
            var productDefinition = BuildProductDefinitionWithEachAttributeTypes();

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
                VariantId = VariantId
            });

            // Assert
            specificationViewModel.Should().NotBeNull();
            specificationViewModel.Groups.Should().NotBeEmpty();

            var group = specificationViewModel.Groups.First();
            group.Title.ShouldBeEquivalentTo("group 1");

            var textAttribute = group.Attributes.Find(x => x.Title == "attr Text");
            textAttribute.Should().NotBeNull();
            textAttribute.Value.ShouldBeEquivalentTo("test2");
        }

        [Test]
        public void WHEN_variant_not_found_SHOULD_return_product_specifications()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag
                {
                    {"attrText", "test1"}
                },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag
                        {
                            {"attrText", "test2"}
                        }
                    }
                }
            };
            var productDefinition = BuildProductDefinitionWithEachAttributeTypes();

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
                VariantId = "invalid"
            });

            // Assert
            specificationViewModel.Should().NotBeNull();
            specificationViewModel.Groups.Should().NotBeEmpty();

            var group = specificationViewModel.Groups.First();
            group.Title.ShouldBeEquivalentTo("group 1");

            var textAttribute = group.Attributes.Find(x => x.Title == "attr Text");
            textAttribute.Should().NotBeNull();
            textAttribute.Value.ShouldBeEquivalentTo("test1");
        }

        [Test]
        public void WHEN_empty_propertybag_SHOULD_return_null()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag(),
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };

            var productDefinition = BuildProductDefinitionWithEachAttributeTypes();

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
                VariantId = VariantId
            });

            // Assert
            specificationViewModel.Should().BeNull();
        }

        [Test]
        public void WHEN_propertybag_with_all_basic_attribute_types_SHOULD_return_correct_specification()
        {
            // Arrange
            var datetimeNow = DateTime.Now;
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag
                {
                    {"attrText", "test1"},
                    {"attrBoolean", true},
                    {"attrCurrency", 10.5},
                    {"attrDatetime", datetimeNow},
                    {"attrDecimal", 10.5},
                    {"attrNumber", 10},
                },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };
            var productDefinition = BuildProductDefinitionWithEachAttributeTypes();

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
            });

            // Assert
            specificationViewModel.Should().NotBeNull();
            specificationViewModel.Groups.Should().NotBeEmpty();
            var group = specificationViewModel.Groups.First();
            group.Title.ShouldBeEquivalentTo("group 1");

            var textAttribute = group.Attributes.Find(x => x.Title == "attr Text");
            textAttribute.Should().NotBeNull();
            textAttribute.Value.ShouldBeEquivalentTo("test1");

            var booleanAttribute = group.Attributes.Find(x => x.Title == "attr Boolean");
            booleanAttribute.Should().NotBeNull();
            booleanAttribute.Value.ShouldBeEquivalentTo("True");

            var currencyAttribute = group.Attributes.Find(x => x.Title == "attr Currency");
            currencyAttribute.Should().NotBeNull();
            currencyAttribute.Value.ShouldBeEquivalentTo(string.Format("{0}", 10.5));

            var datetimeAttribute = group.Attributes.Find(x => x.Title == "attr Datetime");
            datetimeAttribute.Should().NotBeNull();
            datetimeAttribute.Value.ShouldBeEquivalentTo(datetimeNow.ToShortDateString());

            var decimalAttribute = group.Attributes.Find(x => x.Title == "attr Decimal");
            decimalAttribute.Should().NotBeNull();
            decimalAttribute.Value.ShouldBeEquivalentTo(string.Format("{0}", 10.5));

            var numberAttribute = group.Attributes.Find(x => x.Title == "attr Number");
            numberAttribute.Should().NotBeNull();
            numberAttribute.Value.ShouldBeEquivalentTo("10");
        }

        [Test]
        public void WHEN_propertybag_with_single_value_lookup_attribute_SHOULD_return_correct_specification()
        {
            // Arrange
            var propertyName = GetRandom.String(32);
            var lookupName = GetRandom.String(32);

            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag { {propertyName, "test1"} },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };

            var productDefinition = BuildProductDefinitionForLookupWithOneAttribute("attr Lookup", propertyName, lookupName);

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
            });

            // Assert
            var group = specificationViewModel.Groups.First();
            var attribute = group.Attributes.First();
            attribute.Title.ShouldBeEquivalentTo("attr Lookup");
            attribute.Value.ShouldBeEquivalentTo("test 1");
            attribute.PropertyName.ShouldBeEquivalentTo(propertyName);
        }

        [Test]
        public void WHEN_propertybag_with_multiple_value_lookup_attribute_SHOULD_return_correct_specification()
        {
            // Arrange
            var propertyName = GetRandom.String(32);
            var lookupName = GetRandom.String(32);

            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag { { propertyName, "test1|test2" } },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };

            var productDefinition = BuildProductDefinitionForLookupWithOneAttribute("attr Lookup", propertyName, lookupName);
           
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
            });

            // Assert
            var group = specificationViewModel.Groups.First();
            var attribute = group.Attributes.First();
            attribute.Title.ShouldBeEquivalentTo("attr Lookup");
            attribute.Value.ShouldBeEquivalentTo("test 1, test 2");
            attribute.PropertyName.ShouldBeEquivalentTo(propertyName);
        }

        [Test]
        public void SHOULD_return_specification_with_custom_formatting()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag { { "attrWithCustomResource", "test1" } },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };

            var productDefinition = BuildProductDefinitionWithOneAttribute("attr WithCustomResource", "attrWithCustomResource", PropertyDataType.Text);
          
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
            });

            // Assert
            var group = specificationViewModel.Groups.First();
            var attribute = group.Attributes.First();
            attribute.Title.ShouldBeEquivalentTo("attr WithCustomResource");
            attribute.Value.ShouldBeEquivalentTo("test1 customText");
            attribute.PropertyName.ShouldBeEquivalentTo("attrWithCustomResource");
        }

        [Test]
        public void WHEN_empty_value_SHOULD_return_no_specification_attribute()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag { { "attr1", "" } },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };

            var productDefinition = BuildProductDefinitionWithOneAttribute("attr 1", "attr1", PropertyDataType.Text);
           
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
            });

            // Assert
            specificationViewModel.Groups.Should().BeEmpty();
        }

        [Test]
        public void WHEN_excluded_group_SHOULD_return_no_group()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag
                {
                    {"attr1", "test1"},
                },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };

            var productDefinition = BuildProductDefinitionWithOneAttribute("attr 1", "attr1", PropertyDataType.Text);
            productDefinition.PropertyGroups.First().Name = "Default"; //Excluded Group

            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
            });

            // Assert
            specificationViewModel.Groups.Should().BeEmpty();
        }

        [Test]
        public void WHEN_excluded_property_SHOULD_return_no_property()
        {
            // Arrange
            var product = new Overture.ServiceModel.Products.Product
            {
                PropertyBag = new PropertyBag
                {
                    {"ProductDefinition", "test1"}, //Excluded Property
                },
                Variants = new List<Variant>
                {
                    new Variant
                    {
                        Id = VariantId,
                        PropertyBag = new PropertyBag()
                    }
                }
            };

            var productDefinition = BuildProductDefinitionWithOneAttribute("attr 1", "ProductDefinition", PropertyDataType.Text);
            productDefinition.PropertyGroups.First().Name = "GroupTest";
            
            var productSpecificationsViewService = _container.CreateInstance<ProductSpecificationsViewService>();

            // Act 
            var specificationViewModel = productSpecificationsViewService.GetProductSpecificationsViewModel(new GetProductSpecificationsParam
            {
                Product = product,
                ProductDefinition = productDefinition,
            });

            // Assert
            specificationViewModel.Groups.Should().BeEmpty();
        }

        private void SetupProductRepository(Overture.ServiceModel.Products.Product product, ProductDefinition definition)
        {
            var productRepository = new Mock<IProductRepository>();

            productRepository.Setup(r => r.GetProductAsync(It.Is<GetProductParam>(param => param.ProductId == ProductId))).ReturnsAsync(product);
            productRepository.Setup(r => r.GetProductDefinitionAsync(It.Is<GetProductDefinitionParam>(param => param.Name == product.DefinitionName))).ReturnsAsync(definition);

            _container.Use(productRepository);
        }

        private ProductDefinition BuildProductDefinitionWithEachAttributeTypes()
        {
            var productDefinition = new ProductDefinition
            {
                Name = "productDefinition1",
                PropertyGroups = new List<ProductPropertyDefinitionGroup>
                {
                    new ProductPropertyDefinitionGroup
                    {
                        Name = "group1",
                        DisplayName = new LocalizedString(new Dictionary<string, string>{{"en-US", "group 1"}}),
                        Properties = new List<ProductPropertyDefinition>
                        {
                            BuildAttributeDefinition("attr Text", "attrText", PropertyDataType.Text),
                            BuildAttributeDefinition("attr Boolean", "attrBoolean", PropertyDataType.Boolean),
                            BuildAttributeDefinition("attr Currency", "attrCurrency", PropertyDataType.Currency),
                            BuildAttributeDefinition("attr Datetime", "attrDatetime", PropertyDataType.DateTime),
                            BuildAttributeDefinition("attr Decimal", "attrDecimal", PropertyDataType.Decimal),
                            BuildAttributeDefinition("attr Number", "attrNumber", PropertyDataType.Number),
                        }
                    }
                }
            };

            return productDefinition;
        }

        private ProductDefinition BuildProductDefinitionWithOneAttribute(string displayName, string name, PropertyDataType type)
        {
            var productDefinition = new ProductDefinition
            {
                Name = "productDefinition1",
                PropertyGroups = new List<ProductPropertyDefinitionGroup>
                {
                    new ProductPropertyDefinitionGroup
                    {
                        Name = "group1",
                        DisplayName = new LocalizedString(new Dictionary<string, string>{{"en-US", "group 1"}}),
                        Properties = new List<ProductPropertyDefinition>
                        {
                            BuildAttributeDefinition(displayName, name, type),
                        }
                    }
                }
            };
            return productDefinition;
        }

        private ProductPropertyDefinition BuildAttributeDefinition(string displayName, string name, PropertyDataType type)
        {
            return new ProductPropertyDefinition
            {
                DisplayName = new LocalizedString(new Dictionary<string, string> { { "en-US", displayName } }),
                DataType = type,
                PropertyName = name,
                Localizable = true
            };
        }

        private ProductDefinition BuildProductDefinitionForLookupWithOneAttribute(string displayName, string propertyName, string lookupName)
        {
            var productDefinition = new ProductDefinition
            {
                Name = "productDefinition1",
                PropertyGroups = new List<ProductPropertyDefinitionGroup>
                {
                    new ProductPropertyDefinitionGroup
                    {
                        Name = "group1",
                        DisplayName = new LocalizedString(new Dictionary<string, string>{{"en-US", "group 1"}}),
                        Properties = new List<ProductPropertyDefinition>
                        {
                            BuildAttributeDefinitionForLookup(displayName, propertyName, lookupName),
                        }
                    }
                }
            };
            return productDefinition;
        }

        private ProductPropertyDefinition BuildAttributeDefinitionForLookup(string displayName, string propertyName, string lookupName)
        {
            return new ProductPropertyDefinition
            {
                DisplayName = new LocalizedString(new Dictionary<string, string> { { "en-US", displayName } }),
                DataType = PropertyDataType.Lookup,
                PropertyName = propertyName,
                Localizable = true,
                LookupDefinition = new ProductLookupDefinition
                {
                    LookupName = lookupName
                }
            };
        }
    }
}
