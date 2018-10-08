using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class UrlFormatterFormat
    {
        [Test]
        public void WHEN_Url_Contains_Spaces_SHOULD_Replace_With_Dashes()
        {
            // Arrange
            const string url = "red wooden chair";
            const string expected = "red-wooden-chair";

            // Act
            var formattedUrl = UrlFormatter.Format(url);

            // Assert
            formattedUrl.Should().Be(expected);
        }

        [Test]
        [TestCase("A <big> wooden chair/stool + a black & white table", "a-big-wooden-chairstool-a-black-white-table")]
        [TestCase("Une première étable à bois conçue pour l'hiver.", "une-premiere-etable-a-bois-concue-pour-lhiver")]
        [TestCase("Çześć, jak się masz", "czesc-jak-sie-masz")]
        [TestCase("casse-tête", "casse-tete")]
        [TestCase(" apple ", "apple")]
        public void WHEN_Url_Contains_Invalid_Characters_SHOULD_Remove_Them(string url, string expected)
        {
            // Arrange

            // Act
            var formattedUrl = UrlFormatter.Format(url);

            // Assert
            formattedUrl.Should().Be(expected);
        }
    }
}
