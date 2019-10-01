using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers.Dam;
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

            // Act
            Action action = () => new ConventionBasedDamProvider(siteConfiguration.Object);

            // Assert
            action.ShouldNotThrow();
        }
    }
}
