using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class CultureInfoEqualityComparerGetHashCode : BaseTestForAutocreatedSutOfType<CultureInfoEqualityComparer>
    {
        [TestCase("en-US")]
        [TestCase("en")]
        [TestCase("fr-CA")]
        [TestCase(null)]
        public void WHEN_always_SHOULD_return_0(string cultureName)
        {
            //Arrange
            var culture = string.IsNullOrWhiteSpace(cultureName) ? null : CultureInfo.GetCultureInfo(cultureName);
            var equalityComparer = Container.CreateInstance<CultureInfoEqualityComparer>();

            //Act
            var result = equalityComparer.GetHashCode(culture);

            //Assert
            result.Should().Equals(0);
        }

    }
}
