using System;
using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsBlockHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class IfGtBlockHelper_HelperFunction
    {
        private Func<object, string> _template;
        [SetUp]
        public void Setup()
        {
            var helpers = new IfGtBlockHelper();
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            _template = Handlebars.Compile("{{#if_gt V1 V2}}Primary{{else}}Inverse{{/if_gt}}");
        }


        [Test]
        public void WHEN_V1_Is_Greater_Than_V2_SHOULD_Render_Primary_Template()
        {
            var result = _template.Invoke(new TestViewModel() {V1 = 2, V2 = 1});

            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_V1_Is_Less_Than_V2_SHOULD_Render_Inverse_Template()
        {
            var result = _template.Invoke(new TestViewModel() {V1 = 1, V2 = 2});

            result.Should().Be("Inverse");
        }

        [Test]
        public void WHEN_Parameters_Are_Non_Decimal_Numbers_SHOULD_Parse_Correctly()
        {
            var result = _template.Invoke(new TestNumericTypeViewModel {V1 = 2, V2 = 1});

            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_Parameters_Are_Not_Numeric_SHOULD_Throw_Exception()
        {
            _template.Invoking(t => t.Invoke(new TestStringViewModel {V1 = "one", V2 = "two"})).ShouldThrow<ArgumentException>();
        }
    }
}
