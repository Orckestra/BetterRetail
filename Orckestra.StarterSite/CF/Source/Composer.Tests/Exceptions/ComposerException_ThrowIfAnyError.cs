using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.Exceptions
{
    [TestFixture]
    public class ComposerException_ThrowIfAnyError
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [Test]
        public void WHEN_errors_list_is_null_SHOULD_throw_argument_null_exception()
        {
            // Arrange
            List<ErrorViewModel> errors = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new ComposerException(errors).ThrowIfAnyError());
        }

        [Test]
        public void WHEN_errors_list_is_empty_SHOULD_not_throw_exception()
        {
            // Arrange
            var errors = new List<ErrorViewModel>();

            // Act and Assert
            Assert.DoesNotThrow(() => new ComposerException(errors).ThrowIfAnyError());
        }

        [Test]
        public void WHEN_errors_list_contains_one_error_SHOULD_throw_composer_exception()
        {
            // Arrange
            var errors = new List<ErrorViewModel>
            {
                new ErrorViewModel()
            };

            // Act and Assert
            Assert.Throws<ComposerException>(() => new ComposerException(errors).ThrowIfAnyError());
        }

        [Test]
        public void WHEN_errors_list_contains_many_errors_SHOULD_throw_composer_exception()
        {
            // Arrange
            var errors = new List<ErrorViewModel>
            {
                new ErrorViewModel(),
                new ErrorViewModel()
            };

            // Act and Assert
            Assert.Throws<ComposerException>(() => new ComposerException(errors).ThrowIfAnyError());
        }

        [Test]
        public void WHEN_errors_list_is_not_empty_SHOULD_throw_composer_exception_which_contains_list_of_errors()
        {
            // Arrange
            var errors = new List<ErrorViewModel>
            {
                new ErrorViewModel(),
                new ErrorViewModel()
            };

            // Act
            try
            {
                new ComposerException(errors).ThrowIfAnyError();
            }
            catch (ComposerException e)
            {
                // Assert
                e.Errors.Should().BeSameAs(errors);
            }
        }
    }
}
