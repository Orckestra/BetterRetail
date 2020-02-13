using System.Globalization;
using System.Threading;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using HandlebarsDotNet;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Composer.ViewEngine.HandleBarsHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class LocalizeFormatHelper_HelperFunction
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WHEN_Localized_Value_Is_Not_Found_SHOULD_Render_Formatted_Key_Hint()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsNothing();
            var helpers = new LocalizeFormatHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
            
            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var template = Handlebars.Compile("{{localizeFormat CategoryName KeyName}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            result.Should().Be(string.Format("[{0}.{1}]", categoryName, keyName), "Because this is the formatted value hint");
        }

        [Test]
        public void WHEN_Localized_Value_Is_Found_SHOULD_Render_LocalizedValue()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new LocalizeFormatHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string cultureName = "en-CA";
            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var template = Handlebars.Compile("{{localizeFormat CategoryName KeyName}}");

            //Act
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            result.Should().Be(string.Format("{0}:{1}.{2}", cultureName, categoryName, keyName), "Because this is the mocked value");
        }

        [Test]
        [TestCase("en-CA")]
        [TestCase("fr-FR")]
        [TestCase("es-MX")]
        public void WHEN_Using_The_Helper_The_Culture_From_The_UIThread_SHOULD_Be_Used(string expectedCultureName)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new LocalizeFormatHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string categoryName  = GetRandom.String(32);
            string keyName       = GetRandom.String(32);
            var template = Handlebars.Compile("{{localizeFormat CategoryName KeyName}}");

            //Act
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(expectedCultureName);
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().Be(string.Format("{0}:{1}.{2}", expectedCultureName, categoryName, keyName), "Because no better solution was available at this time");
        }

        [Test]
        public void WHEN_Called_With_Variables_SHOULD_Render_Same_As_With_Literals()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new LocalizeFormatHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var templateVariables = Handlebars.Compile("{{localizeFormat CategoryName KeyName}}");
            var templateLiteral   = Handlebars.Compile("{{localizeFormat '"+categoryName+"' '"+keyName+"'}}");

            //Act
            var resultFromVariables = templateVariables.Invoke(new { CategoryName = categoryName, KeyName = keyName });
            var resultFromLiteral   = templateLiteral.Invoke(null);

            //Assert
            resultFromVariables.Should().BeEquivalentTo(resultFromLiteral);
        }

        [Test]
        [TestCase("ResxLocalizationTest", "StrongAndEmphasized", "This value is <strong>Strong</strong> and <em>emphasized</em> with multiple spaces           ok!")]
        public void WHEN_ValueContains_XHTML_SHOULD_use_SafeString_to_render_XHTML_asis(string categoryName, string keyName, string expectedResult)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            var helpers = new LocalizeFormatHelper(localizationProvider);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
            var template = Handlebars.Compile("{{localizeFormat CategoryName KeyName}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [TestCase("{0}",     "Boby", "Boby")]
        [TestCase("{{0}}",   "Boby", "{0}")]
        [TestCase("{0}",     "{0}",  "{0}")]
        [TestCase("{0}",     "{1}",  "{1}")]
        [TestCase("a{0}",    "Boby", "aBoby")]
        [TestCase("a {0}",   "Boby", "a Boby")]
        [TestCase("{0}b",    "Boby", "Bobyb")]
        [TestCase("{0} b",   "Boby", "Boby b")]
        [TestCase("a{0}b",   "Boby", "aBobyb")]
        [TestCase("a {0} b", "Boby", "a Boby b")]
        [TestCase("{0}{0}",  "Boby", "BobyBoby")]
        [TestCase("{0} {0}", "Boby", "Boby Boby")]
        public void WHEN_Value_Contains_1_Options_SHOULD_replace_with_argument(string localizedValue, object arg0, string expectedResult)
        {
            var localizationProvider = new Mock<ILocalizationProvider>(MockBehavior.Strict);
            localizationProvider
                .Setup(lp => lp.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
                .Returns(localizedValue)
                .Verifiable();

            //Arrange
            var helpers = new LocalizeFormatHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
            var template = Handlebars.Compile("{{localizeFormat 'UseMock' 'UseMock' Arg0}}");

            //Act
            var result = template.Invoke(new { Arg0 = arg0 });

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [TestCase("{0}{1}",      "Boby", "Cool", "BobyCool")]
        [TestCase("{1}{0}",      "Boby", "Cool", "CoolBoby")]
        [TestCase("{0} {1}",     "Boby", "Cool", "Boby Cool")]
        [TestCase("{1} {0}",     "Boby", "Cool", "Cool Boby")]
        [TestCase("a{0}{1}",     "Boby", "Cool", "aBobyCool")]
        [TestCase("a{1}{0}",     "Boby", "Cool", "aCoolBoby")]
        [TestCase("a {0}{1}",    "Boby", "Cool", "a BobyCool")]
        [TestCase("a {1}{0}",    "Boby", "Cool", "a CoolBoby")]
        [TestCase("a{0} {1}",    "Boby", "Cool", "aBoby Cool")]
        [TestCase("a{1} {0}",    "Boby", "Cool", "aCool Boby")]
        [TestCase("a {0} {1}",   "Boby", "Cool", "a Boby Cool")]
        [TestCase("a {1} {0}",   "Boby", "Cool", "a Cool Boby")]
        [TestCase("a{0}{1}b",    "Boby", "Cool", "aBobyCoolb")]
        [TestCase("a{1}{0}b",    "Boby", "Cool", "aCoolBobyb")]
        [TestCase("a {0}{1} b",  "Boby", "Cool", "a BobyCool b")]
        [TestCase("a {1}{0} b",  "Boby", "Cool", "a CoolBoby b")]
        [TestCase("a{0} {1}b",   "Boby", "Cool", "aBoby Coolb")]
        [TestCase("a{1} {0}b",   "Boby", "Cool", "aCool Bobyb")]
        [TestCase("a {0} {1} b", "Boby", "Cool", "a Boby Cool b")]
        [TestCase("a {1} {0} b", "Boby", "Cool", "a Cool Boby b")]
        [TestCase("{0}{1}b",     "Boby", "Cool", "BobyCoolb")]
        [TestCase("{1}{0}b",     "Boby", "Cool", "CoolBobyb")]
        [TestCase("{0}{1} b",    "Boby", "Cool", "BobyCool b")]
        [TestCase("{1}{0} b",    "Boby", "Cool", "CoolBoby b")]
        [TestCase("{0} {1}b",    "Boby", "Cool", "Boby Coolb")]
        [TestCase("{1} {0}b",    "Boby", "Cool", "Cool Bobyb")]
        [TestCase("{0} {1} b",   "Boby", "Cool", "Boby Cool b")]
        [TestCase("{1} {0} b",   "Boby", "Cool", "Cool Boby b")]
        [TestCase("{{0}}{{1}}",  "Boby", "Cool", "{0}{1}")]
        [TestCase("{0}{1} {1} {0}", "Boby", "Cool", "BobyCool Cool Boby")]
        [TestCase("{0}{1}", "{0}", "{1}", "{0}{1}")]
        [TestCase("{0}{1}", "{1}", "{0}", "{1}{0}")]
        public void WHEN_Value_Contains_2_Options_SHOULD_replace_with_argument(string localizedValue, object arg0, object arg1, string expectedResult)
        {
            var localizationProvider = new Mock<ILocalizationProvider>(MockBehavior.Strict);
            localizationProvider
                .Setup(lp => lp.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
                .Returns(localizedValue)
                .Verifiable();

            //Arrange
            var helpers = new LocalizeFormatHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
            var template = Handlebars.Compile("{{localizeFormat 'UseMock' 'UseMock' Arg0 Arg1}}");

            //Act
            var result = template.Invoke(new { Arg0 = arg0, Arg1 = arg1 });

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        [TestCase("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", "A0", "B1", "C2", "D3", "E4", "F5", "G6", "H7", "I8", "J9", "A0B1C2D3E4F5G6H7I8J9")]
        public void WHEN_Value_Contains_10_Options_SHOULD_replace_with_argument(
            string localizedValue, 
            object arg0, 
            object arg1,
            object arg2,
            object arg3,
            object arg4,
            object arg5,
            object arg6,
            object arg7,
            object arg8,
            object arg9, 
            string expectedResult)
        {
            var localizationProvider = new Mock<ILocalizationProvider>(MockBehavior.Strict);
            localizationProvider
                .Setup(lp => lp.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
                .Returns(localizedValue)
                .Verifiable();

            //Arrange
            var helpers = new LocalizeFormatHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
            var template = Handlebars.Compile("{{localizeFormat 'UseMock' 'UseMock' Arg0 Arg1 Arg2 Arg3 Arg4 Arg5 Arg6 Arg7 Arg8 Arg9}}");

            //Act
            var result = template.Invoke(new
            {
                Arg0 = arg0, 
                Arg1 = arg1,
                Arg2 = arg2,
                Arg3 = arg3,
                Arg4 = arg4,
                Arg5 = arg5,
                Arg6 = arg6,
                Arg7 = arg7,
                Arg8 = arg8,
                Arg9 = arg9
            });

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
