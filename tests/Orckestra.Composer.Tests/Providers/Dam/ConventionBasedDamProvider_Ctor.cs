using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;

namespace Orckestra.Composer.Tests.Providers.Dam
{
    [TestFixture]
    public class ConventionBasedDamProviderCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange
            var siteConfiguration = new Mock<ISiteConfiguration>();
            var productMediaSettingsRepository = new Mock<IProductMediaSettingsRepository>();

            // Act
            Action action = () => new ConventionBasedDamProvider(siteConfiguration.Object, productMediaSettingsRepository.Object);

            // Assert
            action.ShouldNotThrow();
        }
    }
}
