using System;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Providers.RegionCode;

namespace Orckestra.Composer.Tests.Providers.RegionCode
{
    [TestFixture]
    public class RegionCodeProviderCtor
    {
        [Test]
        public void WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            // Act
            Action action = () => new RegionCodeProvider();

            // Assert
            action.ShouldNotThrow();
        }
    }
}
