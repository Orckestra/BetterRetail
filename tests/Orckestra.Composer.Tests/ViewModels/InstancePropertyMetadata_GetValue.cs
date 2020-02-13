using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    [TestFixture]
    public class InstancePropertyMetadataGetValue
    {
        [Test]
        public void WHEN_ViewModel_Is_Valid_SHOULD_Pass()
        {
            // Arrange
            var expectedPropertyValue = GetRandom.String(20);

            var propertyInfo = typeof(TestViewModel).GetProperty("Property");
            var sut = new InstancePropertyMetadata(propertyInfo);
            var viewModel = new TestViewModel { Property = expectedPropertyValue };

            // Act
            var result = sut.GetValue(viewModel);

            // Assert
            result.Should().Be(expectedPropertyValue);
        }

        [Test]
        public void WHEN_ViewModel_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var propertyInfo = typeof(TestViewModel).GetProperty("Property");
            var sut = new InstancePropertyMetadata(propertyInfo);

            // Act
            Action action = () => sut.GetValue(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        class TestViewModel : BaseViewModel
        {
            public string Property { get; set; }
        }
    }
}
