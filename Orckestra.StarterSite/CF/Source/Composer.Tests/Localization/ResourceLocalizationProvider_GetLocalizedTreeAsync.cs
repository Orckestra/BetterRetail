using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Tests.Mock;

namespace Orckestra.Composer.Tests.Localization
{
    [TestFixture]
    public class ResourceLocalizationProviderGetLocalizedTreeAsync
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        [TestCase("ResxLocalizationTest")]
        [TestCase("CategoryCustomOnly")]
        [TestCase("CategoryCultureOnly")]
        [TestCase("CategoryNeutralOnly")]
        public async Task WHEN_Passing_Any_Culture_SHOULD_Contain_All_Categories(string expectedCategory)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            CultureInfo culture = CultureInfo.GetCultureInfo("es-US");

            //Act
            LocalizationTree tree = await localizationProvider.GetLocalizationTreeAsync(culture);

            //Assert
            tree.LocalizedCategories.Values.Should().Contain(c => c.CategoryName == expectedCategory);
            tree.LocalizedCategories.Should().ContainKey(expectedCategory.ToLowerInvariant(), "Because Javascript expect lowercase for case insentivity");
        }

        [Test]
        [TestCase("ResxLocalizationTest", "PageTitle")]
        [TestCase("ResxLocalizationTest", "PostalCodeRegexPattern")]
        [TestCase("ResxLocalizationTest", "String1")]
        [TestCase("CategoryNeutralOnly",  "KeyNeutralOnly")]
        [TestCase("CategoryCustomOnly",   "KeyCustomOnly")]
        public async Task WHEN_Passing_Any_Culture_SHOULD_Contain_All_Keys_From_Neutral_Source(string expectedCategory, string expectedKey)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            CultureInfo culture = CultureInfo.GetCultureInfo("es-US");

            //Act
            LocalizationTree tree = await localizationProvider.GetLocalizationTreeAsync(culture);

            //Assert
            tree.LocalizedCategories.Values.Should().Contain(c => c.CategoryName == expectedCategory);
            tree.LocalizedCategories.Should().ContainKey(expectedCategory.ToLowerInvariant(), "Because Javascript expect lowercase for case insentivity");
            tree.LocalizedCategories[expectedCategory.ToLowerInvariant()].LocalizedValues.Should().ContainKey(expectedKey);
        }

        [TestCase("fr-CA", "ResxLocalizationTest", "PageTitle")]
        [TestCase("fr-CA", "ResxLocalizationTest", "PageTitle")]
        [TestCase("fr-CA", "ResxLocalizationTest", "PostalCodeRegexPattern")]
        [TestCase("fr-CA", "ResxLocalizationTest", "String1")]
        [TestCase("fr-CA", "CategoryNeutralOnly",  "KeyNeutralOnly")]
        [TestCase("fr-CA", "CategoryCustomOnly",   "KeyCustomOnly")]
        [TestCase("fr-CA", "CategoryCultureOnly",  "KeyCultureOnly")]
        public async Task WHEN_Passing_A_Given_Culture_SHOULD_Contain_All_Keys_From_Possible_Sources(string cultureName, string expectedCategory, string expectedKey)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);

            //Act
            LocalizationTree tree = await localizationProvider.GetLocalizationTreeAsync(culture);

            //Assert
            tree.LocalizedCategories.Values.Should().Contain(c => c.CategoryName == expectedCategory);
            tree.LocalizedCategories.Should().ContainKey(expectedCategory.ToLowerInvariant(), "Because Javascript expect lowercase for case insentivity");
            tree.LocalizedCategories[expectedCategory.ToLowerInvariant()].LocalizedValues.Should().ContainKey(expectedKey);
        }

        [TestCase("fr-BE", "ResxLocalizationTest", "PageTitle", "Customized Belgium Page Title")]
        [TestCase("fr-CA", "ResxLocalizationTest", "PageTitle", "Québec Page Title")]
        [TestCase("fr-FR", "ResxLocalizationTest", "PageTitle", "Generic French Page Title")]
        [TestCase("es-MX", "ResxLocalizationTest", "PageTitle", "Neutral Page Title")]
        [TestCase("es-MX", "CategoryCustomOnly",   "KeyCustomOnly", "Some Cool Project with some Cool Localization, just to add some cool categories")]
        public async Task WHEN_Passing_A_Given_Culture_SHOULD_Contain_The_Most_Relevant_Value_For_Each_Keys(string cultureName, string category, string key, string expectedValue)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);

            //Act
            LocalizationTree tree = await localizationProvider.GetLocalizationTreeAsync(culture);

            //Assert
            tree.LocalizedCategories.Values.Should().Contain(c => c.CategoryName == category);
            tree.LocalizedCategories.Should().ContainKey(category.ToLowerInvariant(), "Because Javascript expect lowercase for case insentivity");
            tree.LocalizedCategories[category.ToLowerInvariant()].LocalizedValues.Should().ContainKey(key);
            tree.LocalizedCategories[category.ToLowerInvariant()].LocalizedValues[key].Should().BeEquivalentTo(expectedValue);
        }

        [Test]
        public void WHEN_Passing_NULL_Culture_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(async () =>
            {
                LocalizationTree tree = await localizationProvider.GetLocalizationTreeAsync(null);
            });

            //Assert
            exception.ParamName.Should().BeSameAs("culture");
            exception.Message.Should().Contain("culture");
        }

        [Test]
        public async Task WHEN_Passing_Any_Culture_Resulting_LocalizationCategories_SHOULD_Not_Be_Null()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            CultureInfo culture = CultureInfo.GetCultureInfo("es-MX");

            //Act
            LocalizationTree tree = await localizationProvider.GetLocalizationTreeAsync(culture);

            //Assert
            tree.LocalizedCategories.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_Passing_Any_Culture_Resulting_TreeStructure_SHOULD_Not_Be_Null()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            CultureInfo culture = CultureInfo.GetCultureInfo("es-MX");

            //Act
            LocalizationTree tree = await localizationProvider.GetLocalizationTreeAsync(culture);

            //Assert
            tree.Should().NotBeNull();
            tree.LocalizedCategories.Should().NotBeNull();
            foreach (var category in tree.LocalizedCategories.Values)
            {
                category.Should().NotBeNull();
                category.LocalizedValues.Should().NotBeNull();
            }
        }

        [Test]
        public async Task WHEN_Passing_Any_Culture_Resulting_TreeStructure_SHOULD_Be_Cached_ByCulture()
        {
            //Arrange
            CacheProviderFactory.CacheHistMonitor monitor = new CacheProviderFactory.CacheHistMonitor();
            LocalizationTree cachedTree = new LocalizationTree(CultureInfo.CurrentCulture);

            AutoMocker container = new AutoMocker();
            container.Use(CacheProviderFactory.CreateWithMonitor(monitor, cachedTree));
            container.Use(ComposerEnvironmentFactory.Create());

            ILocalizationProvider localizationProvider = container.CreateInstance<ResourceLocalizationProvider>();

            CultureInfo cultureA = CultureInfo.GetCultureInfo("fr-CA");
            CultureInfo cultureB = CultureInfo.GetCultureInfo("fr-FR");

            //Act
            monitor.Reset();
            monitor.CacheMissCount.ShouldBeEquivalentTo(0, "Otherwise this test is irrelevent");
            monitor.CacheHitCount.ShouldBeEquivalentTo(0, "Otherwise this test is irrelevent");

            LocalizationTree tree1A = await localizationProvider.GetLocalizationTreeAsync(cultureA);
            monitor.CacheMissCount.ShouldBeEquivalentTo(1, "First attempt to load the CultureA should cache miss");

            LocalizationTree tree2A = await localizationProvider.GetLocalizationTreeAsync(cultureA);
            monitor.CacheHitCount.ShouldBeEquivalentTo(1, "Second attempt to load the CultureA should cache hit");

            monitor.Reset();
            for (int i = 0; i < 10; i++)
            {
                LocalizationTree tree3A = await localizationProvider.GetLocalizationTreeAsync(cultureA);

            }
            monitor.CacheMissCount.ShouldBeEquivalentTo(0, "Subsequent attempt to load the CultureA should not cache miss");
            monitor.CacheHitCount.Should().BeGreaterOrEqualTo(10, "Subsequent attempt to load the CultureA should cache hit");

            //--
            monitor.Reset();
            LocalizationTree tree1B = await localizationProvider.GetLocalizationTreeAsync(cultureB);
            monitor.CacheMissCount.ShouldBeEquivalentTo(1, "First attempt to load the CultureB should cache miss, key is culture dependant");
            monitor.CacheHitCount.ShouldBeEquivalentTo(0, "First attempt to load the CultureB should not cache hit, key is culture dependant");
        }
    }
}
