using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Repositories;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.Product.Tests.Repositories
{
    [TestFixture]
    public class CategoryRepositoryCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            var overtureClient = new Mock<IComposerOvertureClient>();
            var cacheProvider = new Mock<ICacheProvider>();

            // Act
            Action action = () => new CategoryRepository(overtureClient.Object, cacheProvider.Object);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_Passing_Null_OvertureClient_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var cacheProvider = new Mock<ICacheProvider>();

            // Act
            Action action = () => new CategoryRepository(null, cacheProvider.Object);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_CacheProvider_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var overtureClient = new Mock<IComposerOvertureClient>();

            // Act
            Action action = () => new CategoryRepository(overtureClient.Object, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
