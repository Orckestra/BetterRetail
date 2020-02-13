using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsBlockHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class IfLteBlockHelper_HelperFunction
    {
        [SetUp]
        public void Setup()
        {
            var helpers = new IfLteBlockHelper();
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
        }

        [Test]
        public void When_V1_Is_Less_Or_Equal_V2_SHOULD_Render_Primary_Template()
        {
            var template = Handlebars.Compile("{{#if_lte V1 V2}}Primary{{else}}Inverse{{/if_lte}}");

            var lessThanResult = template.Invoke(new TestViewModel() {V1 = 1, V2 = 2});
            var equalResult = template.Invoke(new TestViewModel() {V1 = 1, V2 = 1});

            lessThanResult.Should().Be("Primary");
            equalResult.Should().Be("Primary");
        }

        [Test]
        public void WHEN_V1_Is_Greater_Than_V2_SHOULD_Render_Inverse_Template()
        {
            var template = Handlebars.Compile("{{#if_lte V1 V2}}Primary{{else}}Inverse{{/if_lte}}");

            var result = template.Invoke(new TestViewModel() {V1 = 2, V2 = 1});

            result.Should().Be("Inverse");
            
        }
    }
}
