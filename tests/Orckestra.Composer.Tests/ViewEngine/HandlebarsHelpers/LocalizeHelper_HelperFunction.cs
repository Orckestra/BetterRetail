using System.Globalization;
using System.Threading;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Composer.ViewEngine.HandleBarsHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class LocalizeHelper_HelperFunction
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
            var helpers = new LocalizeHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
            
            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var template = Handlebars.Compile("{{localize CategoryName KeyName}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            result.Should().Be(string.Format("[{0}.{1}]", categoryName, keyName), "Because this is the formatted value hint");
        }

        [Test]
        public void WHEN_Localized_Value_Is_Found_SHOULD_Render_LocalizedValue()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new LocalizeHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string cultureName = "en-CA";
            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var template = Handlebars.Compile("{{localize CategoryName KeyName}}");

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
            var helpers = new LocalizeHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string categoryName  = GetRandom.String(32);
            string keyName       = GetRandom.String(32);
            var template = Handlebars.Compile("{{localize CategoryName KeyName}}");

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
            var helpers = new LocalizeHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var templateVariables = Handlebars.Compile("{{localize CategoryName KeyName}}");
            var templateLiteral   = Handlebars.Compile("{{localize '"+categoryName+"' '"+keyName+"'}}");

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
            var helpers = new LocalizeHelper(localizationProvider);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
            var template = Handlebars.Compile("{{localize CategoryName KeyName}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
