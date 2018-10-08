using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsBlockHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class IfExistsBlockHelper_HelperFunction
    {
        [SetUp]
        public void Setup()
        {
            var helpers = new IfExistsBlockHelper();
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);
        }
        [Test]
        public void WHEN_Value_Is_Not_Null_SHOULD_Render_Template()
        {
            var template = Handlebars.Compile(@"{{#if_exists Value}}yolo{{/if_exists}}");

            var viewResult = template.Invoke(new TestViewModel {Value = false});

            viewResult.Should().Be("yolo");
        }

        [Test]
        public void WHEN_Value_Is_Null_SHOULD_Render_Blank_Result()
        {
            var template = Handlebars.Compile(@"{{#if_exists Value}}yolo{{/if_exists}}");

            var viewResult = template.Invoke(new TestViewModel{Value = null});

            viewResult.Should().Be("");
        }

        [Test]
        public void WHEN_Incorrect_Number_Of_Arguments_Are_Passed_SHOULD_Throw_HandlebarsException()
        {
            var template = Handlebars.Compile(@"{{#if_exists Value1 Value2}}yolo{{/if_exists}}");

           template.Invoking(t => t.Invoke(new {Value1 = "hello", Value2 = "world"})).ShouldThrow<HandlebarsException>();
        }

        [Test]
        public void WHEN_Value_Is_Null_And_Else_Template_Specified_SHOULD_Render_Else_Block()
        {
            var template = Handlebars.Compile(@"{{#if_exists Value}}yolo{{else}}nolo{{/if_exists}}");

            var viewResult = template.Invoke(new TestViewModel());

            viewResult.Should().Be("nolo");
        }
        private class TestViewModel
        {
            public bool? Value { get; set; } 
        }
    }
}
