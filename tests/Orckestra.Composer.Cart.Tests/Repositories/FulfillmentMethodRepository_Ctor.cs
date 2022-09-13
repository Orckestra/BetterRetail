using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Cart.Repositories;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    extern alias occ;
    [TestFixture]
    public class FulfillmentMethodRepositoryCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            var overtureclient = new Mock<IComposerOvertureClient>();
            var cacheProvider = new Mock<occ::Orckestra.Overture.Caching.ICacheProvider>();

            // Act
            Action action = () => new FulfillmentMethodRepository(overtureclient.Object, cacheProvider.Object);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_Passing_Null_OvertureClient_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var cacheProvider = new Mock<occ::Orckestra.Overture.Caching.ICacheProvider>();

            // Act
            Action action = () => new FulfillmentMethodRepository(null, cacheProvider.Object);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_CacheProvider_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var overtureclient = new Mock<IComposerOvertureClient>();

            // Act
            Action action = () => new FulfillmentMethodRepository(overtureclient.Object, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
