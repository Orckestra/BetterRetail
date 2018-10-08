using System;
using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    [TestFixture]
    public class InstancePropertyMetadataCtor
    {
        public string Property { get; set; }

        [DisplayName("Dummy")]
        public int PropertyWithDisplayName { get; set; }

        private string _propertyNoGetter;
        public string PropertyNoGetter
        {
            set { _propertyNoGetter = value; }
        }

        private const string FormattingCategory = "testcategory";
        private const string FormattingKey = "testkey";

        [Formatting(FormattingCategory, FormattingKey)]
        public string PropertyWithFormattingAttribute { get; set; }

        [Lookup(LookupType.Product, "Brand")]
        public string LookupProperty { get; set; }

        [Lookup(LookupType.Customer, "Size", delimiter:"-")]
        public string LookupProperty2 { get; set; }

        [Test]
        public void WHEN_PropertyInfo_Is_Valid_SHOULD_Pass()
        {
            // Arrange
            var propertyInfo = GetType().GetProperty("Property");

            // Act
            var sut = new InstancePropertyMetadata(propertyInfo);

            // Assert
            sut.PropertyName.Should().Be("Property");
            sut.DisplayName.Should().Be("Property");
            sut.PropertyType.Should().Be(typeof(string));
            sut.FormattableProperty.Should().BeFalse();
            sut.LookupProperty.Should().BeFalse();
        }

        [Test]
        public void WHEN_PropertyInfo_Has_A_DisplayNameAttribute_SHOULD_Use_It_As_Display_Name()
        {
            // Arrange
            var propertyInfo = GetType().GetProperty("PropertyWithDisplayName");

            // Act
            var sut = new InstancePropertyMetadata(propertyInfo);

            // Assert
            sut.PropertyName.Should().Be("PropertyWithDisplayName");
            sut.DisplayName.Should().Be("Dummy");
            sut.PropertyType.Should().Be(typeof(int));
        }

        [Test]
        public void WHEN_PropertyInfo_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange

            // Act
            Action instantiate = () => new InstancePropertyMetadata(null);

            // Assert
            instantiate.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_PropertyInfo_Has_No_Getter_SHOULD_Throw_ArgumentException()
        {
            // Arrange
            var noGetter = GetType().GetProperty("PropertyNoGetter");

            // Act
            Action instantiateNoGetter = () => new InstancePropertyMetadata(noGetter);

            // Assert
            instantiateNoGetter.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_Property_Has_FormattingAttribute_SHOULD_Formatting_Properties_Be_Set()
        {
            // Arrange
            var property = GetType().GetProperty("PropertyWithFormattingAttribute");

            // Act
            var sut = new InstancePropertyMetadata(property);

            // Assert
            sut.FormattableProperty.Should().BeTrue();
            sut.PropertyFormattingCategory.Should().Be(FormattingCategory);
            sut.PropertyFormattingKey.Should().Be(FormattingKey);
        }

        [Test]
        public void WHEN_LookupAttributePresent_SHOULD_Lookup_Properties_Be_Set()
        {
            var property = GetType().GetProperty("LookupProperty");

            var sut = new InstancePropertyMetadata(property);

            sut.LookupProperty.Should().BeTrue();
            sut.LookupName.Should().Be("Brand");
            sut.LookupDelimiter.Should().Be(", ");
        }

        [Test]
        public void WHEN_Delimiter_Set_For_LookupAttribute_LookupDelimiter_SHOULD_Be_Equivalent()
        {
            var property = GetType().GetProperty("LookupProperty2");

            var sut = new InstancePropertyMetadata(property);

            sut.LookupDelimiter.Should().Be("-");
        }
    }
}
