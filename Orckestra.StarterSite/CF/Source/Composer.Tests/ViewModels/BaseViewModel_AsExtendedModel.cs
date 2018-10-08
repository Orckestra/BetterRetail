using System;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels
{
    [TestFixture]
    public class BaseViewModel_AsExtendedModel
    {
        [Test]
        public void WHEN_parameter_type_does_not_extend_the_view_model_type_SHOULD_throw_InvalidOperationException()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();

            // Act
            var exception =
                Assert.Throws<InvalidOperationException>(
                    () => viewModel.AsExtensionModel<INonExtensionViewModel>());

            // Assert
            exception.Should().NotBeNull();
        }

        [Test]
        public void WHEN_parameter_type_extends_the_view_model_type_SHOULD_return_ExtensionOf_type_of_view_model()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();

            // Act
            var extendedModel = viewModel.AsExtensionModel<IExtensionViewModel>();

            // Assert
            extendedModel.Should().NotBeNull();
        }
    }
}