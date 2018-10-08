using FizzWare.NBuilder.Generators;
using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsBlockHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class IfIsLocalizedBlockHelper_HelperFunction
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("NotAKnownCategory")]
        public void WHEN_Category_Is_Not_Found_SHOULD_Render_Inverse_Template(string categoryName)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            var helpers = new IfIsLocalizedBlockHelper(localizationProvider);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string keyName = GetRandom.String(32);
            var template = Handlebars.Compile("{{#if_localized CategoryName KeyName}}Primary{{else}}Inverse{{/if_localized}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().Be("Inverse");
        }

        [Test]
        [TestCase("ResxLocalizationTest", "ThisValueIsEmpty")]
        public void WHEN_Category_Key_Resolve_To_Empty_SHOULD_Render_Inverse_Template(string categoryName, string keyName)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            var helpers = new IfIsLocalizedBlockHelper(localizationProvider);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            var template = Handlebars.Compile("{{#if_localized CategoryName KeyName}}Primary{{else}}Inverse{{/if_localized}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().Be("Inverse");
        }

        [Test]
        [TestCase("ResxLocalizationTest", "ThisValueIsWhitespace")]
        public void WHEN_Category_Key_Resolve_To_WhiteSpaces_SHOULD_Render_Primary_Template(string categoryName, string keyName)
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
            var helpers = new IfIsLocalizedBlockHelper(localizationProvider);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            var template = Handlebars.Compile("{{#if_localized CategoryName KeyName}}Primary{{else}}Inverse{{/if_localized}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_Localized_Value_Is_Not_Found_Or_Null_Or_Empty_Found_SHOULD_Render_Inverse_Template()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsNothing();
            var helpers = new IfIsLocalizedBlockHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string categoryName = GetRandom.String(32);
            string keyName = GetRandom.String(32);

            var template = Handlebars.Compile("{{#if_localized CategoryName KeyName}}Primary{{else}}Inverse{{/if_localized}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().Be("Inverse");
        }

        [Test]
        public void WHEN_Localized_Value_Is_Not_Null_Or_Empty_Found_SHOULD_Render_Primary_Template()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new IfIsLocalizedBlockHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string categoryName = GetRandom.String(32);
            string keyName = GetRandom.String(32);

            var template = Handlebars.Compile("{{#if_localized CategoryName KeyName}}Primary{{else}}Inverse{{/if_localized}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_Called_With_Variables_SHOULD_Render_Same_As_With_Literals()
        {
            //Arrange
            var localizationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new IfIsLocalizedBlockHelper(localizationProvider.Object);
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var templateVariables = Handlebars.Compile("{{#if_localized CategoryName KeyName}}Primary{{else}}Inverse{{/if_localized}}");
            var templateLiteral   = Handlebars.Compile("{{#if_localized '"+categoryName+"' '"+keyName+"'}}Primary{{else}}Inverse{{/if_localized}}");

            //Act
            var resultFromVariables = templateVariables.Invoke(new { CategoryName = categoryName, KeyName = keyName });
            var resultFromLiteral   = templateLiteral.Invoke(null);

            //Assert
            resultFromVariables.Should().BeEquivalentTo(resultFromLiteral);
        }
    }
}
