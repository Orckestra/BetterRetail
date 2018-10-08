using System;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Product.Providers;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.Product.Tests.Providers
{
    [TestFixture]
    public class ConfInventoryLocationProvider_GetInventoryLocationProviderId
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
        }

        [TearDown]
        public void TearDown()
        {
            _container.VerifyAll();
        }

        [Test]
        public void WHEN_Calling_GetInventoryLocationProviderId_SHOULD_Not_Throw()
        {
            //Arrange
            IInventoryLocationProvider inventoryLocationProvider =
                _container.CreateInstance<ConfigurationInventoryLocationProvider>();

            // Act
            Action action = async () => await inventoryLocationProvider.GetDefaultInventoryLocationIdAsync();

            // Assert
            action.ShouldNotThrow();
        }
    }
}
