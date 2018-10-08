using System.Globalization;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Tests.Mock;

namespace Orckestra.Composer.Tests.Localization
{
    [TestFixture]
    public class HandlebarViewResourceProviderTest
    {
        private AutoMocker _container;
        private ILocalizationProvider _localizationProvider;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(CacheProviderFactory.CreateForLocalizationTree());
            _container.Use(ComposerEnvironmentFactory.Create());

            _localizationProvider = _container.CreateInstance<ResourceLocalizationProvider>();
        }

        [TestCase]
        public void WHEN_Handlebar_View_Retrive_Not_Defined_Culture_Key_SHOULD_Return_Local_Resource_Neutral_Value()
        {
            var value = _localizationProvider.GetLocalizedString(
                    new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest",
                        Key         = "PageTitle",
                        CultureInfo = CultureInfo.CreateSpecificCulture("en-US")
                    }
                );

            Assert.AreEqual(value, "Neutral Page Title");
        }

        [TestCase]
        public void WHEN_Handlebar_View_Retreive_Generic_Language_SHOULD_Return_Local_Resource_French_Generic_Value()
        {
            var value = _localizationProvider.GetLocalizedString(
                    new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest",
                        Key         = "PageTitle",
                        CultureInfo = CultureInfo.CreateSpecificCulture("fr-FR")
                    }
                );

            Assert.AreEqual(value, "Generic French Page Title");
        }

        [TestCase]
        public void WHEN_Handlebar_View_Retrieve_Culture_Specified_Key_SHOULD_Return_Local_Resource_French_Canadian_Value()
        {
            var value = _localizationProvider.GetLocalizedString(
                    new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest",
                        Key         = "PageTitle",
                        CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA")
                    }
                );

            Assert.AreEqual(value, "Québec Page Title");
        }

        [TestCase]
        public void WHEN_Handlebar_View_Retrieve_From_Non_Exising_ResourceFile_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                    new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest2",
                        Key         = "PageTitle",
                        CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA")
                    }    
                );
            value.Should().BeNull();
        }

        [TestCase]
        public void WHEN_Handlebar_View_Retrive_Not_Defined_Culture_And_Key_Does_Not_Exists_SHOULD_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                    new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest",
                        Key         = "InvalidKey",
                        CultureInfo = CultureInfo.CreateSpecificCulture("en-US")
                    }
                );

            value.Should().BeNull();
        }

        [TestCase]
        public void WHEN_Handlebar_View_Retreive_Generic_Language_And_Key_Does_Not_Exists_SHOULD_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest",
                        Key         = "InvalidKey",
                        CultureInfo = CultureInfo.CreateSpecificCulture("fr-FR")
                    }
                );

            value.Should().BeNull();
        }

        [TestCase]
        public void WHEN_Handlebar_View_Retrieve_Culture_Specified_And_Key_Does_Not_Exists_SHOULD_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                    new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest",
                        Key         = "InvalidKey",
                        CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA")
                    }
                );

            value.Should().BeNull();
        }
    }
}
