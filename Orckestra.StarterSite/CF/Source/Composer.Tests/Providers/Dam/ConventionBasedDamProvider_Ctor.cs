using System;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Providers.Dam;

namespace Orckestra.Composer.Tests.Providers.Dam
{
    [TestFixture]
    public class ConventionBasedDamProviderCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Arrange


            // Act
            Action action = () => new ConventionBasedDamProvider();

            // Assert
            action.ShouldNotThrow();
        }
    }
}
