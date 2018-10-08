using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    [TestFixture]
    public class InstancePropertyMetadataSetValue
    {
        [Test]
        public void WHEN_ViewModel_And_Value_Are_Valid_SHOULD_Pass()
        {
            // Arrange
            var expectedPropertyValue = GetRandom.String(20);

            var propertyInfo = typeof(TestViewModel).GetProperty("StringProperty");
            var sut = new InstancePropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel();

            // Act
            sut.SetValue(viewModel, expectedPropertyValue);

            // Assert
            viewModel.StringProperty.Should().Be(expectedPropertyValue);
        }

        [Test]
        public void WHEN_ViewModel_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("StringProperty");
            var sut = new InstancePropertyMetadata(propertyInfo);

            // Act
            Action action = () => sut.SetValue(null, "test");

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Value_Is_Null_And_Property_Is_Reference_Type_SHOULD_Pass()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("StringProperty");
            var sut = new InstancePropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel();

            // Act
            sut.SetValue(viewModel, null);

            // Assert
            viewModel.StringProperty.Should().Be(null);
        }

        [Test]
        public void WHEN_Value_Is_Null_And_Property_Is_Value_Type_SHOULD_Skip()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("IntProperty");
            var sut = new InstancePropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel();

            // Act
            sut.SetValue(viewModel, null);

            // Assert
            viewModel.IntProperty.Should().Be(default(int));
        }

        class TestViewModel : BaseViewModel
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }
    }
}
