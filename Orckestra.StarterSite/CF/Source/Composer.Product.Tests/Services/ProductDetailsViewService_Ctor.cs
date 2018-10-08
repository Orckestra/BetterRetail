using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.Product.Tests.Services
{
    [TestFixture]
    public class ProductDetailsViewServiceCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            var productViewModelFactory = new Mock<IProductViewModelFactory>();
            var productUrlProvider = new Mock<IProductUrlProvider>();

            Action action = () => new ProductViewService(productViewModelFactory.Object, productUrlProvider.Object);

            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_ProductViewModelFactory_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            var productUrlProvider = new Mock<IProductUrlProvider>();

            //Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ProductViewService(null, productUrlProvider.Object);
            });
        }

        [Test]
        public void WHEN_ProductUrlProvider_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            //Arrange
            var productViewModelFactory = new Mock<IProductViewModelFactory>();

            //Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ProductViewService(productViewModelFactory.Object, null);
            });
        }
    }
}
