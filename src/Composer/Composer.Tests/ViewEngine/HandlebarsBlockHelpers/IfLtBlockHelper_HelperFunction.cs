using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsBlockHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class IfLtBlockHelper_HelperFunction
    {
        [SetUp]
        public void Setup()
        {
            var helpers = new IfLtBlockHelper();
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
        }

        [Test]
        public void When_V1_Is_Less_Than_V2_SHOULD_Render_Primary_Template()
        {
            var template = Handlebars.Compile("{{#if_lt V1 V2}}Primary{{else}}Inverse{{/if_lt}}");

            var result = template.Invoke(new TestViewModel() {V1 = 1, V2 = 2});

            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_V1_Is_Greater_Than_V2_SHOULD_Render_Inverse_Template()
        {
            var template = Handlebars.Compile("{{#if_lt V1 V2}}Primary{{else}}Inverse{{/if_lt}}");

            var result = template.Invoke(new TestViewModel() {V1 = 2, V2 = 1});

            result.Should().Be("Inverse");
        }
    }
}
