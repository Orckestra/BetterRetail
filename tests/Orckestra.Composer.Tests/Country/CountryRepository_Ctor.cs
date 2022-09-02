using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Country;
using Orckestra.Overture.Caching;

namespace Orckestra.Composer.Tests.Country
{
    [TestFixture]
    public class CountryRepositoryCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            Mock<IComposerOvertureClient> overtureClient = new Mock<IComposerOvertureClient>();
            Mock<ICacheProvider> cacheProvider = new Mock<ICacheProvider>();

            // Act
            Action action = () => new CountryRepository(overtureClient.Object, cacheProvider.Object);

            // Assert
            action.ShouldNotThrow();
        }

        [Test]
        public void WHEN_Passing_Null_OvertureClient_SHOULD_Throw_Exception()
        {
            Mock<ICacheProvider> cacheProvider = new Mock<ICacheProvider>();

            // Act
            Action action = () => new CountryRepository(null, cacheProvider.Object);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Passing_Null_CacheProvider_SHOULD_Throw_Exception()
        {
            Mock<IComposerOvertureClient> overtureClient = new Mock<IComposerOvertureClient>();

            // Act
            Action action = () => new CountryRepository(overtureClient.Object, null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
