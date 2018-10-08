using System;
using FluentAssertions;
using NUnit.Framework;

namespace Orckestra.Composer.Tests.TypeExtensions.TypeExtensions
{
    [TestFixture]
    public class TypeExtensionGetDefaultValue
    {
        [Test]
        public void WHEN_type_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            Type type = null;

            //Act
            var action = new Action(() => Composer.TypeExtensions.TypeExtensions.GetDefaultValue(type));

            //Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestCase(typeof(int), 0)]
        [TestCase(typeof(float), 0.0f)]
        [TestCase(typeof(double), 0.0)]
        [TestCase(typeof(bool), false)]
        public void WHEN_type_is_byVal_SHOULD_return_value_default_value(Type type, object expectedValue)
        {
            //Arrange

            //Act
            var defaultValue = Composer.TypeExtensions.TypeExtensions.GetDefaultValue(type);

            //Assert
            defaultValue.Should().Be(expectedValue);
            //defaultValue.ShouldEqual(expectedValue);
        }

        [Test]
        public void WHEN_type_is_decimal_SHOULD_return_zero_decimal()
        {
            //Arrange
            var type = typeof (decimal);
            var expectedValue = 0.0m;

            //Act
            var defaultValue = Composer.TypeExtensions.TypeExtensions.GetDefaultValue(type);

            //Assert
            defaultValue.Should().Be(expectedValue);
            //defaultValue.ShouldEqual(expectedValue);
        }

        [Test]
        public void WHEN_type_is_Guid_SHOULD_return_empty_Guid()
        {
            //Arrange
            var type = typeof(Guid);
            var expectedValue = Guid.Empty;

            //Act
            var defaultValue = Composer.TypeExtensions.TypeExtensions.GetDefaultValue(type);

            //Assert
            defaultValue.Should().Be(expectedValue);
            //defaultValue.ShouldEqual(expectedValue);
        }

        [TestCase(typeof(int?))]
        [TestCase(typeof(double?))]
        [TestCase(typeof(decimal?))]
        [TestCase(typeof(bool?))]
        [TestCase(typeof(string))]
        [TestCase(typeof(object))]
        [TestCase(typeof(DateTime?))]
        [TestCase(typeof(Guid?))]
        [TestCase(typeof(Composer.ComposerHost))]
        public void WHEN_type_is_ByRef_SHOULD_return_null(Type type)
        {
            //Arrange

            //Act
            var defaultValue = Composer.TypeExtensions.TypeExtensions.GetDefaultValue(type);

            //Assert
            defaultValue.Should().BeNull();
        } 
    }
}
