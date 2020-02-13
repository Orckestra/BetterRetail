using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.Cart.Tests.Repositories
{
    [TestFixture]
    public class FulfillmentMethodRepositoryCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            var overtureclient = new Mock<IOvertureClient>();
            var cacheProvider = new Mock<ICacheProvider>();

            // Act
            Action action = () => new FulfillmentMethodRepository(overtureclient.Object, cacheProvider.Object);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_Passing_Null_OvertureClient_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var cacheProvider = new Mock<ICacheProvider>();

            // Act
            Action action = () => new FulfillmentMethodRepository(null, cacheProvider.Object);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_CacheProvider_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var overtureclient = new Mock<IOvertureClient>();

            // Act
            Action action = () => new FulfillmentMethodRepository(overtureclient.Object, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
