using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CultureInfoEqualityComparer_Equals : BaseTestForAutocreatedSutOfType<CultureInfoEqualityComparer>
    {
        [TestCase("en-US", "en-us")]
        [TestCase("fr-CA", "fr-CA")]
        [TestCase("en", "EN")]
        [TestCase(null, null)]
        public void WHEN_culture_match_SHOULD_return_true(string culture1Name, string culture2Name)
        {
            //Arrange
            var culture1 = string.IsNullOrWhiteSpace(culture1Name) ? null : CultureInfo.GetCultureInfo(culture1Name);
            var culture2 = string.IsNullOrWhiteSpace(culture2Name) ? null : CultureInfo.GetCultureInfo(culture2Name);
            var equalityComparer = Container.CreateInstance<CultureInfoEqualityComparer>();

            //Act
            var result = equalityComparer.Equals(culture1, culture2);

            //Assert
            result.Should().BeTrue();
        }

        [TestCase("en-US", "en")]
        [TestCase("fr", "fr-CA")]
        [TestCase(null, "fr-CA")]
        [TestCase("en-US", null)]
        public void WHEN_culture_does_not_match_SHOULD_return_false(string culture1Name, string culture2Name)
        {
            //Arrange
            var culture1 = string.IsNullOrWhiteSpace(culture1Name) ? null : CultureInfo.GetCultureInfo(culture1Name);
            var culture2 = string.IsNullOrWhiteSpace(culture2Name) ? null : CultureInfo.GetCultureInfo(culture2Name);
            var equalityComparer = Container.CreateInstance<CultureInfoEqualityComparer>();

            //Act
            var result = equalityComparer.Equals(culture1, culture2);

            //Assert
            result.Should().BeFalse();
        }
    }
}
