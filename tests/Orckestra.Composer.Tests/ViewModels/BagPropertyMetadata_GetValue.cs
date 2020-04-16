using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    [TestFixture]
    public class BagPropertyMetadataGetValue
    {
        [Test]
        public void WHEN_ViewModel_Is_Valid_SHOULD_Pass()
        {
            // Arrange
            var expectedValue = GetRandom.DateTime();
            var propertyInfo = typeof(TestViewModel).GetProperty("Property");
            var sut = new BagPropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel();
            sut.SetValue(viewModel, expectedValue);

            // Act
            var result = sut.GetValue(viewModel);

            // Assert
            result.Should().Be(expectedValue);
        }

        [Test]
        public void WHEN_Property_Has_Not_Been_Initialized_SHOULD_Return_Default()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("Property");
            var sut = new BagPropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel();

            // Act
            var result = sut.GetValue(viewModel);

            // Assert
            result.Should().Be(default(DateTime));
        }

        [Test]
        public void WHEN_ViewModel_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("Property");
            var sut = new BagPropertyMetadata(propertyInfo);

            // Act
            Action action = () => sut.GetValue(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        class TestViewModel : BaseViewModel
        {
            public DateTime Property { get; set; }
        }
    }
}
