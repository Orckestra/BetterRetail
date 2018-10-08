using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class CategoryViewServiceCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            var viewModelMapper    = new Mock<IViewModelMapper>();
            var categoryRepository = new Mock<ICategoryRepository>();

            // Act
            Action action = () => new CategoryViewService(viewModelMapper.Object, categoryRepository.Object);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_ViewModelMapper_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var categoryRepository = new Mock<ICategoryRepository>();

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new CategoryViewService(null, categoryRepository.Object);
            });

            //Assert
            exception.ParamName.Should().BeSameAs("viewModelMapper");
        }

        [Test]
        public void WHEN_CategoryRepository_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var viewModelMapper    = new Mock<IViewModelMapper>();

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new CategoryViewService(viewModelMapper.Object, null);
            });

            //Assert
            exception.ParamName.Should().BeSameAs("categoryRepository");
        }
    }
}
