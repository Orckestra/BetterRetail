using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    [TestFixture]
    public class BagPropertyMetadataSetValue
    {
        [Test]
        public void WHEN_ViewModel_And_Value_Are_Valid_SHOULD_Pass()
        {
            // Arrange
            var expectedPropertyValue = GetRandom.String(20);

            var propertyInfo = typeof(TestViewModel).GetProperty("StringProperty");
            var sut = new BagPropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel();

            // Act
            sut.SetValue(viewModel, expectedPropertyValue);

            // Assert
            viewModel.Bag.Should().ContainKey("StringProperty");
            viewModel.Bag["StringProperty"].Should().Be(expectedPropertyValue);
        }

        [Test]
        public void WHEN_ViewModel_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("StringProperty");
            var sut = new BagPropertyMetadata(propertyInfo);

            // Act
            Action action = () => sut.SetValue(null, "test");

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Value_Is_Null_SHOULD_Assign_Null()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("StringProperty");
            var sut = new BagPropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel();

            // Act
            sut.SetValue(viewModel, null);

            // Assert
            viewModel.Bag.Should().ContainKey("StringProperty");
            viewModel.Bag["StringProperty"].Should().Be(null);
        }

        class TestViewModel : BaseViewModel
        {
            public string StringProperty { get; set; }
        }
    }
}
