using System.Globalization;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Tests.Mock;

namespace Orckestra.Composer.Tests.Localization
{
    [TestFixture]
    public class PostalCodeRegexPatternTest
    {
        private ILocalizationProvider _localizationProvider;

        [SetUp]
        public void SetUp()
        {
            _localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
        }

        [TestCase("J5A0G5", "fr-CA")]
        [TestCase("J5A 0G5", "fr-CA")]
        [TestCase("H3K1N8", "fr-CA")]
        [TestCase("H3K 1N8", "fr-CA")]
        [TestCase("H0H0H0", "fr-CA")]
        [TestCase("H0H 0H0", "fr-CA")]
        [TestCase("J5A0G5", "en-CA")]
        [TestCase("J5A 0G5", "en-CA")]
        [TestCase("H3K1N8", "en-CA")]
        [TestCase("H3K 1N8", "en-CA")]
        [TestCase("H0H0H0", "en-CA")]
        [TestCase("H0H 0H0", "en-CA")]
        public void Assert_That_Valid_Canadian_Postal_Code_SHOULD_Succeed_With_Canadian_Localization(string postalCode, string culture)
        {
            var postalCodeRegexPattern = _localizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category    = "ResxLocalizationTest",
                Key         = "PostalCodeRegexPattern",
                CultureInfo = CultureInfo.GetCultureInfo(culture)
            });

            var matches = Regex.Match(postalCode, postalCodeRegexPattern);
            
            Assert.That(matches.Success);
        }

        [TestCase("", "fr-CA")]
        [TestCase("   ", "fr-CA")]
        [TestCase("1", "fr-CA")]
        [TestCase("123", "fr-CA")]
        [TestCase("123456", "fr-CA")]
        [TestCase("123 456", "fr-CA")]
        [TestCase("", "en-CA")]
        [TestCase("   ", "en-CA")]
        [TestCase("1", "en-CA")]
        [TestCase("123", "en-CA")]
        [TestCase("123456", "en-CA")]
        [TestCase("123 456", "en-CA")]
        public void Assert_That_Invalid_Canadian_Postal_Code_SHOULD_Not_Succeed_With_Canadian_Localization(string postalCode, string culture)
        {
            var postalCodeRegexPattern = _localizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "ResxLocalizationTest",
                Key      = "PostalCodeRegexPattern",
                CultureInfo = CultureInfo.GetCultureInfo(culture),
            });

            var matches = Regex.Match(postalCode, postalCodeRegexPattern);

            Assert.That(!matches.Success);
        }
    }
}
