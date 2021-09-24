using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    [TestFixture(Category = "ViewModelMapper")]
	public class ViewModelMapperMapTo : BaseTest
	{
        public Composer.ViewModels.ViewModelMapper ViewModelMapper { get; set; }
        private bool _prevLogDisabledState;

        public override void SetUp()
        {
            base.SetUp();
            _prevLogDisabledState = LogProvider.IsDisabled;
            LogProvider.IsDisabled = true;

            InitMetadataRegistry();
            ViewModelMapper = Container.CreateInstance<Composer.ViewModels.ViewModelMapper>();
        }

        [TearDown]
        public void TearDown()
        {
            LogProvider.IsDisabled = _prevLogDisabledState;
        }

        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Pass()
        {
            // Arrange
            var validProduct = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            var result = ViewModelMapper.MapTo<TestProductViewModel>(validProduct, validEnglishCulture);

            // Assert

            // Verify simple properties
            result.Name.Should().Be("Chair");
            result.Brand.Should().Be("AmericanTire");
            result.Price.Should().Be(9.99);
            result.Width.Should().Be(null);
            result.Height.Should().Be(120);

            // Verify propertybag properties
            result.Colour.Should().Be("Brown");
            result.Description.Should().Be("An exquisite chair.");
            result.CustomCategory.Should().NotBeNull();
            var customCategory = result.CustomCategory;

            customCategory.Id.Should().Be(2);
            customCategory.Title.Should().Be("Custom");

            // Verify complex type properties
            result.Category.Should().NotBeNull();
            var category = result.Category;
            category.Id.Should().Be(42);
            category.Title.Should().Be("Sports");
            category.ParentCategory.Should().NotBeNull();

            var parentCategory = category.ParentCategory;
            parentCategory.Id.Should().Be(1);
            parentCategory.Title.Should().Be("Fitness");

            // Verify array properties
            result.ChildProducts.Should().NotBeNull();
            result.ChildProducts.Should().HaveCount(2);
            result.ChildProducts[0].Brand.Should().Be("FloorMart");
            result.ChildProducts[1].Brand.Should().Be("GenericCompany");

            // Verify list properties
            result.Tags.Should().NotBeNull();
            result.Tags.Count.Should().Be(2);
            result.Tags[0].Should().Be("Outdoor");
            result.Tags[1].Should().Be("Indoor");
        }

        [Test]
        public void WHEN_Source_Object_Is_Empty_SHOULD_Pass()
        {
            // Arrange
            var emptyProduct = new TestProduct();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            var result = ViewModelMapper.MapTo<TestProductViewModel>(emptyProduct, validEnglishCulture);

            // Assert
            result.Should().NotBeNull();
        }

		[Test]
        public void WHEN_Source_Is_Null_SHOULD_Return_Null()
		{
            // Arrange
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
		    var result = ViewModelMapper.MapTo<TestProductViewModel>(null, validEnglishCulture);

            // Assert
		    result.Should().BeNull();
		}

	    [Test]
        public void WHEN_Culture_Is_Null_Or_Empty_SHOULD_Throw_An_ArgumentException()
	    {
	        // Arrange
            var validProduct = TestProductFactory.CreateValid();

            // Act
            Action nullCulture = () => ViewModelMapper.MapTo<TestProductViewModel>(validProduct, (string) null);
            Action emptyCulture = () => ViewModelMapper.MapTo<TestProductViewModel>(validProduct, string.Empty);

            // Assert
	        nullCulture.ShouldThrow<ArgumentException>();
	        emptyCulture.ShouldThrow<ArgumentException>();
	    }

	    [Test]
        public void WHEN_Culture_Is_Invalid_SHOULD_Throw_A_CultureNotFoundException()
	    {
	        // Arrange
            var validProduct = TestProductFactory.CreateValid();

            // Act
            Action dummyCulture = () => ViewModelMapper.MapTo<TestProductViewModel>(validProduct, "dummy");

            // Assert
	        dummyCulture.ShouldThrow<CultureNotFoundException>();
	    }

        [Test]
        public void WHEN_Culture_Is_Not_Registered_SHOULD_Not_Map_At_All()
        {
            // Arrange
            var validProduct = TestProductFactory.CreateValid();
            var chineseCulture = CultureInfo.GetCultureInfo("zh-CN");

            // Act
            var result = ViewModelMapper.MapTo<TestProductViewModel>(validProduct, chineseCulture);

            // Assert
            result.Name.Should().BeNull();
        }

        [Test]
        public void WHEN_Having_A_Subclass_To_Baseclass_SHOULD_Map_It()
        {
            // Arrange
            var validProduct = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            var result = ViewModelMapper.MapTo<ProductViewModelWithSubclass>(validProduct, validEnglishCulture);

            // Assert
            result.Category.Should().NotBeNull();
            result.Category.Id.Should().Be(42);
        }

        [Test]
        public void WHEN_Property_Name_Do_Not_Match_Case_SHOULD_Not_Map_It()
        {
            // Arrange
            var insensitiveProperties = new ProductWithCaseInsensitiveProperties
            {
                brand = "chair",
                Price = 9.99
            };
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            var result = ViewModelMapper.MapTo<TestProductViewModel>(insensitiveProperties, validEnglishCulture);

            // Assert
            result.Brand.Should().Be(default(string));
            result.Price.Should().Be(insensitiveProperties.Price);
        }

        [Test]
        public void WHEN_MapToAttribute_Used_SHOULD_Map_To_Specified_Property()
        {
            //Arrange
            var model = TestProductFactory.CreateValid();
            var culture = CultureInfo.GetCultureInfo("en-CA");

            //Act
            var result = ViewModelMapper.MapTo<ProductViewModelWithMapToAttribute>(model, culture);

            //Assert
            result.Should().NotBeNull();
            result.UnrelatedProperty.Should().NotBeNull();
            result.UnrelatedProperty.Should().HaveCount(model.Name.Count);
        }

        [Test]
        public void WHEN_Going_From_LocalizedString_To_Long_SHOULD_Not_Map_It()
        {
            // Arrange
            var validProduct = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            Action invalidLocalizedString = () => ViewModelMapper.MapTo<ProductViewModelWithInvalidLocalizedStringOutput>(validProduct, validEnglishCulture);

            // Assert
            invalidLocalizedString.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void WHEN_Types_Mismatch_SHOULD_Throw_InvalidOperationException()
        {
            // Arrange
            var productionWithWrongPropertyType = new ProductWithWrongPropertyType
            {
                Brand = 1
            };
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            Action wrongPropertyType = () => ViewModelMapper.MapTo<TestProductViewModel>(productionWithWrongPropertyType, validEnglishCulture);

            // Assert
            wrongPropertyType.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void WHEN_Types_Mismatch_In_PropertyBag_SHOULD_Throw_InvalidOperationException()
        {
            // Arrange
            var wrongTypeInPropertyBag = new ProductWithWrongPropertyTypeInPropertyBag
            {
                PropertyBag = new PropertyBag(new Dictionary<string, object>
                {
                    { "Colour", 9 }
                })
            };
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            Action typesMismatch = () => ViewModelMapper.MapTo<TestProductViewModel>(wrongTypeInPropertyBag, validEnglishCulture);

            // Assert
            typesMismatch.ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void WHEN_All_Properties_Are_Not_Mapped_SHOULD_Succeed()
        {
            // Arrange
            var noProperties = new ProductWithoutProperties();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            var result = ViewModelMapper.MapTo<TestProductViewModel>(noProperties, validEnglishCulture);

            // Assert
            result.Should().NotBeNull();
            result.Description.Should().Be(default(string));
            result.Brand.Should().Be(default(string));
            result.Name.Should().Be(default(string));
            result.Price.Should().Be(default(double));
            result.Colour.Should().Be(default(string));
            result.Width.Should().Be(null);
            result.Height.Should().Be(null);
            result.Category.Should().BeNull();
            result.CustomCategory.Should().BeNull();
            result.ChildProducts.Should().BeNull();
            result.Tags.Should().BeNull();
        }

        [Test]
        public void WHEN_Metadata_Attribute_Is_Defined_SHOULD_Transfer_Between_Bags()
        {
            // Arrange
            var product = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            var result = ViewModelMapper.MapTo<TestProductViewModel>(product, validEnglishCulture);

            // Assert
            result.Should().NotBeNull();
            result.Bag["CustomProperty"].Should().Be(5);
            result.Category.Bag["CustomName"].Should().Be("Custom sports");
        }

        [Test]
        public void WHEN_MapToAttribute_Is_Combined_With_IViewModelMetadata_SHOULD_Set_From_Specified_Bag_Field()
        {
            
            // Arrange
            var product = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");

            // Act
            var result = ViewModelMapper.MapTo<TestProductViewModel>(product, validEnglishCulture);

            // Assert
            result.Should().NotBeNull();
            result.Bag["MappedViewModelBagProperty"].Should().Be("I'm a mapped field!");
        }


        [Test]
        public void WHEN_Format_Attribute_Is_Defined_SHOULD_Call_Format()
        {
            // Arrange
            var product = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");
            var viewModelPropertyFormatterMock = Container.GetMock<IViewModelPropertyFormatter>();
            
            // Act
            ViewModelMapper.MapTo<TestProductViewModel>(product, validEnglishCulture, null);

            // Assert
            viewModelPropertyFormatterMock.Verify(m => m.Format(It.IsAny<DateTime>(),
                It.Is<IPropertyMetadata>(md => md.PropertyFormattingCategory=="TestCategory" && md.PropertyFormattingKey=="Date"),
                It.IsAny<CultureInfo>()));
        }

        [Test]
        public void WHEN_LookupAttribute_Present_SHOULD_Call_LookupService()
        {
            var product = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");
            var lookupServiceMock = Container.GetMock<ILookupService>();

            ViewModelMapper.MapTo<TestProductViewModel>(product, validEnglishCulture);

            lookupServiceMock.Verify(m => m.GetLookupDisplayNameAsync(It.IsAny<GetLookupDisplayNameParam>()));
        }

        [Test]
        public void WHEN_LookupAttribute_Set_SHOULD_Set_Value_From_Lookup()
        {
            var product = TestProductFactory.CreateValid();
            var validEnglishCulture = CultureInfo.GetCultureInfo("en-CA");
            var lookupServiceMock = Container.GetMock<ILookupService>();
            lookupServiceMock.Setup(
                m =>
                    m.GetLookupDisplayNameAsync(It.IsAny<GetLookupDisplayNameParam>()))
                    .ReturnsAsync("X-Large");
            var vm = ViewModelMapper.MapTo<TestProductViewModel>(product, validEnglishCulture);

            vm.Size.Should().Be("X-Large");
        }

        private void InitMetadataRegistry()
        {
            var registry = new ViewModelMetadataRegistry();
            registry.LoadViewModelMetadataInAssemblyOf<ViewModelMapperMapTo>();
            Container.Use<IViewModelMetadataRegistry>(registry);
        }

        #region Product classes
        class ProductWithCaseInsensitiveProperties
        {
            // ReSharper disable once InconsistentNaming (we test explicitly for property with wrong casing)
            public string brand { get; set; }
            public double Price { get; set; }
        }

        class ProductWithWrongPropertyType
        {
            public int Brand { get; set; }
        }

        class ProductWithWrongPropertyTypeInPropertyBag
        {
            public PropertyBag PropertyBag { get; set; }
        }

        class ProductWithoutProperties
        {

        }
        #endregion

        #region ProductViewModel classes
        class ProductViewModelWithInvalidLocalizedStringOutput : BaseViewModel
        {
            public long Name { get; set; }
        }

        class ProductViewModelWithSubclass : BaseViewModel
        {
            public TestBaseCategory Category { get; set; }
        }

        class ProductViewModelWithMapToAttribute : BaseViewModel
        {
            [MapTo("Name")]
            public Dictionary<string, string> UnrelatedProperty { get; set; }
        }
        #endregion
    }
}