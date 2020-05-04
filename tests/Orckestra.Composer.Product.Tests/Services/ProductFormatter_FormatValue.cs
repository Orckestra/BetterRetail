using System.Collections.Generic;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Overture.ServiceModel.Metadata;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class ProductFormatterFormatValue
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(GetLookups());
            _container.Use(GetLocalizationProvider());
        }

        public void WHEN_Passing_Unknown_PropertyName_For_Unknown_DataType_SHOULD_Fallback_To_BasePropertyTypeKey_For_Text()
        {
            //Arrange
            ProductFormatter productFormatter = _container.CreateInstance<ProductFormatter>();
            ProductPropertyDefinition ppd = new ProductPropertyDefinition
            {
                PropertyName = GetRandom.String(32),
                DataType     = (PropertyDataType)int.MinValue
            };
            object value = new object();

            //Act
            string formatted = productFormatter.FormatValue(ppd, value, GetRandomCulture());

            //Assert
            formatted.Should().BeEquivalentTo("Default Localized Text Format", "This is the default fallback for unknown DataTypes");
        }
        
        [Test]
        [TestCase(PropertyDataType.Boolean, true,            "Default Localized True Boolean Format")]
        [TestCase(PropertyDataType.Boolean, false,           "Default Localized False Boolean Format")]
        [TestCase(PropertyDataType.Boolean, "not a boolean", "Default Localized Text Format")] //Fallback to text
        public void WHEN_Passing_Unknown_PropertyName_For_Boolean_SHOULD_Format_Using_BasePropertyTypeKey(PropertyDataType dataType, object value, string expectedFormatted)
        {
            //Arrange
            ProductFormatter productFormatter = _container.CreateInstance<ProductFormatter>();
            ProductPropertyDefinition ppd = new ProductPropertyDefinition
            {
                PropertyName = GetRandom.String(32),
                DataType     = PropertyDataType.Boolean
            };

            //Act
            string formatted = productFormatter.FormatValue(ppd, value, GetRandomCulture());

            //Assert
            formatted.Should().BeEquivalentTo(expectedFormatted);
        }

        [Test]
        [TestCase(PropertyDataType.Lookup, "Default Localized Lookup Format")]
        public void WHEN_Passing_Unknown_PropertyName_For_Lookup_SHOULD_Format_Using_BasePropertyTypeKey(PropertyDataType dataType, string expectedFormatted)
        {
            //Arrange
            ProductFormatter productFormatter = _container.CreateInstance<ProductFormatter>();
            ProductPropertyDefinition ppd = new ProductPropertyDefinition
            {
                PropertyName = GetRandom.String(32),
                DataType     = dataType,
                LookupDefinition = new ProductLookupDefinition
                {
                    LookupName = GetRandom.String(32)
                }
            };
            object value = new object();

            //Act
            string formatted = productFormatter.FormatValue(ppd, value, GetRandomCulture());

            //Assert
            formatted.Should().BeEquivalentTo(expectedFormatted);
        }

        [Test]
        [TestCase(PropertyDataType.Currency, "Default Localized Currency Format")]
        [TestCase(PropertyDataType.Number, "Default Localized Number Format")]
        [TestCase(PropertyDataType.DateTime, "Default Localized DateTime Format")]
        [TestCase(PropertyDataType.Decimal, "Default Localized Decimal Format")]
        [TestCase(PropertyDataType.Text, "Default Localized Text Format")]
        public void WHEN_Passing_Unknown_PropertyName_For_Known_DataType_SHOULD_Format_Using_BasePropertyTypeKey(PropertyDataType dataType, string expectedFormatted)
        {
            //Arrange
            ProductFormatter productFormatter = _container.CreateInstance<ProductFormatter>();
            ProductPropertyDefinition ppd = new ProductPropertyDefinition
            {
                PropertyName = GetRandom.String(32),
                DataType = dataType
            };
            object value = new object();

            //Act
            string formatted = productFormatter.FormatValue(ppd, value, GetRandomCulture());

            //Assert
            formatted.Should().BeEquivalentTo(expectedFormatted);
        }

        [Test]
        [TestCase(PropertyDataType.Currency,        "Known Localized Format")]
        [TestCase(PropertyDataType.Number,          "Known Localized Format")]
        [TestCase(PropertyDataType.DateTime,        "Known Localized Format")]
        [TestCase(PropertyDataType.Decimal,         "Known Localized Format")]
        [TestCase(PropertyDataType.Text,            "Known Localized Format")]
        [TestCase((PropertyDataType)int.MaxValue, "Known Localized Format")]
        public void WHEN_Passing_Known_PropertyName_SHOULD_Use_Known_Format(PropertyDataType dataType, string expectedFormatted)
        {
            //Arrange
            ProductFormatter productFormatter = _container.CreateInstance<ProductFormatter>();
            ProductPropertyDefinition ppd = new ProductPropertyDefinition
            {
                PropertyName = "KnownPropertyName",
                DataType     = dataType
            };
            object value = new object();

            //Act
            string formatted = productFormatter.FormatValue(ppd, value, GetRandomCulture());

            //Assert
            formatted.Should().BeEquivalentTo(expectedFormatted);
        }

        [Test]
        [TestCase(PropertyDataType.Lookup, "Known Localized Format")]
        public void WHEN_Passing_Known_PropertyName_For_Lookups_SHOULD_Use_Known_Format(PropertyDataType dataType, string expectedFormatted)
        {
            //Arrange
            ProductFormatter productFormatter = _container.CreateInstance<ProductFormatter>();
            ProductPropertyDefinition ppd = new ProductPropertyDefinition
            {
                PropertyName = "KnownPropertyName",
                DataType = dataType,
                LookupDefinition = new ProductLookupDefinition
                {
                    LookupName = GetRandom.String(32)
                }
            };
            object value = new object();

            //Act
            string formatted = productFormatter.FormatValue(ppd, value, GetRandomCulture());

            //Assert
            formatted.Should().BeEquivalentTo(expectedFormatted);
        }

        [Test]
        [TestCase(PropertyDataType.Boolean, true,            "Known Localized True")]
        [TestCase(PropertyDataType.Boolean, false,           "Known Localized False")]
        [TestCase(PropertyDataType.Boolean, "not a boolean", "Known Localized Format")]
        public void WHEN_Passing_Known_PropertyName_For_Boolean_SHOULD_Use_Known_Format(PropertyDataType dataType, object value, string expectedFormatted)
        {
            //Arrange
            ProductFormatter productFormatter = _container.CreateInstance<ProductFormatter>();
            ProductPropertyDefinition ppd = new ProductPropertyDefinition
            {
                PropertyName = "KnownPropertyName",
                DataType     = PropertyDataType.Boolean
            };

            //Act
            string formatted = productFormatter.FormatValue(ppd, value, GetRandomCulture());

            //Assert
            formatted.Should().BeEquivalentTo(expectedFormatted);
        }

        #region Mock
        private CultureInfo GetRandomCulture()
        {
            CultureInfo[] all = CultureInfo.GetCultures(CultureTypes.AllCultures);
            int rndIndex = GetRandom.PositiveInt(all.Length-1);

            return all[rndIndex];
        }

        private List<Lookup> GetLookups()
        {
            List<Lookup> dummyLookups = new List<Lookup>();


            return dummyLookups;
        }

        private Mock<ILocalizationProvider> GetLocalizationProvider()
        {
            Dictionary<string,string> dummyFormats = new Dictionary<string, string>
            {
                {"ProductSpecificationsPropertyFormatting.KnownPropertyName",            "Known Localized Format"},
                {"ProductSpecificationsPropertyFormatting.KnownPropertyNameTrue",        "Known Localized True"},
                {"ProductSpecificationsPropertyFormatting.KnownPropertyNameFalse",       "Known Localized False"},
                //-
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeLookup",       "Default Localized Lookup Format"},
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeNumber",       "Default Localized Number Format"},
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeText",         "Default Localized Text Format"},
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeDateTime",     "Default Localized DateTime Format"},
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeDecimal",      "Default Localized Decimal Format"},
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeCurrency",     "Default Localized Currency Format"},
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeBooleanTrue",  "Default Localized True Boolean Format"},
                {"ProductSpecificationsPropertyFormatting.BasePropertyTypeBooleanFalse", "Default Localized False Boolean Format"},
            };


            Mock<ILocalizationProvider> localizationProvider = new Mock<ILocalizationProvider>(MockBehavior.Strict);

            localizationProvider
                .Setup(lp => lp.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
                .Returns<GetLocalizedParam>(param =>
                {
                    string key = string.Format("{0}.{1}", param.Category, param.Key);
                    if (!dummyFormats.TryGetValue(key, out string value))
                    {
                        value = null;
                    }
                    return value;
                })
                .Verifiable();

            return localizationProvider;
        }
        #endregion
    }
}
