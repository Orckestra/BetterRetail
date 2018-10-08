using System;
using System.Globalization;
using System.Threading;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Composer.ViewEngine;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsHelpers
{
    // ReSharper disable once InconsistentNaming
    class HandlebarsHelpers_Localize
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WHEN_Localized_Value_Is_Not_Found_SHOULD_Render_Formatted_Key_Hint()
        {
            //Arrange
            var localisationProvider = LocalizationProviderFactory.CreateKnowsNothing();
            var helpers = new HandleBarsHelpers(localisationProvider.Object);
            Handlebars.RegisterHelper(HandleBarsHelpers.LocalizeTagName, helpers.Localize);
            
            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var template = Handlebars.Compile("{{localize CategoryName KeyName}}");

            //Act
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            result.Should().Be(String.Format("[{0}.{1}]", categoryName, keyName), "Because this is the formatted key hint");
        }

        [Test]
        public void WHEN_Localized_Value_Is_Found_SHOULD_Render_LocalizedValue()
        {
            //Arrange
            var localisationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new HandleBarsHelpers(localisationProvider.Object);
            Handlebars.RegisterHelper(HandleBarsHelpers.LocalizeTagName, helpers.Localize);

            string cultureName = "en-CA";
            string categoryName = GetRandom.String(32);
            string keyName      = GetRandom.String(32);
            var template = Handlebars.Compile("{{localize CategoryName KeyName}}");

            //Act
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            result.Should().Be(String.Format("{0}:{1}.{2}", cultureName, categoryName, keyName), "Because this is the mocked value");
        }

        [Test]
        [TestCase("en-CA")]
        [TestCase("fr-FR")]
        [TestCase("es-MX")]
        public void WHEN_Using_The_Helper_The_Culture_From_The_UIThread_SHOULD_Be_Used(string expectedCultureName)
        {
            //Arrange
            var localisationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new HandleBarsHelpers(localisationProvider.Object);
            Handlebars.RegisterHelper(HandleBarsHelpers.LocalizeTagName, helpers.Localize);

            string categoryName  = GetRandom.String(32);
            string keyName       = GetRandom.String(32);
            var template = Handlebars.Compile("{{localize CategoryName KeyName}}");

            //Act
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(expectedCultureName);
            var result = template.Invoke(new { CategoryName = categoryName, KeyName = keyName });

            //Assert
            result.Should().Be(String.Format("{0}:{1}.{2}", expectedCultureName, categoryName, keyName), "Because no better solution was available at this time");
        }

        [Test]
        public void WHEN_Called_With_Variables_SHOULD_Render_Same_As_With_Literals()
        {
            //Arrange
            var localisationProvider = LocalizationProviderFactory.CreateKnowsItAll();
            var helpers = new HandleBarsHelpers(localisationProvider.Object);
            Handlebars.RegisterHelper(HandleBarsHelpers.LocalizeTagName, helpers.Localize);

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
    }
}
