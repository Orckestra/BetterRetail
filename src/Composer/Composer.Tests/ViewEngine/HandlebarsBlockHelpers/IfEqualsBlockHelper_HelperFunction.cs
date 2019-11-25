using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using HandlebarsDotNet;
using NUnit.Framework;
using Orckestra.Composer.ViewEngine.HandleBarsBlockHelpers;

namespace Orckestra.Composer.Tests.ViewEngine.HandlebarsBlockHelpers
{
    // ReSharper disable once InconsistentNaming
    internal sealed class IfEqualsBlockHelper_HelperFunction
    {
        private Func<object, string> _template;
        [SetUp]
        public void Setup()
        {
            var helpers = new IfEqualsBlockHelper();
            Handlebars.RegisterHelper(helpers.HelperName, helpers.HelperFunction);

            _template = Handlebars.Compile("{{#if_eq V1 V2}}Primary{{else}}Inverse{{/if_eq}}");
        }


        [Test]
        public void WHEN_Int_Values_Same_Value_SHOULD_Render_Primary_Template()
        {
            //Arrange
            int sameValue = GetRandom.Int();
            int v1 = sameValue;
            int v2 = sameValue;

            //Act
            var result = _template.Invoke(new {V1 = v1, V2 = v2});

            //Assert
            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_Int_Values_Different_Values_SHOULD_Render_Inverse_Template()
        {
            //Arrange
            int v1 = GetRandom.Int();
            int v2 = GetRandom.Int();
            while (v2 == v1) { v2 = GetRandom.Int(); }

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Inverse");
        }

        [Test]
        public void WHEN_Decimal_Values_Same_Value_SHOULD_Render_Primary_Template()
        {
            //Arrange
            int sameValue = GetRandom.Int();
            int v1 = sameValue;
            int v2 = sameValue;

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_Decimal_Values_Different_Values_SHOULD_Render_Inverse_Template()
        {
            //Arrange
            decimal v1 = GetRandom.Decimal();
            decimal v2 = GetRandom.Decimal();
            while (v2 == v1) { v2 = GetRandom.Decimal(); }

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Inverse");
        }

        [Test]
        public void WHEN_Boolean_Values_Same_Value_SHOULD_Render_Primary_Template()
        {
            //Arrange
            bool sameValue = GetRandom.Boolean();
            bool v1 = sameValue;
            bool v2 = sameValue;

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_Boolean_Values_Different_Values_SHOULD_Render_Inverse_Template()
        {
            //Arrange
            bool v1 = GetRandom.Boolean();
            bool v2 = !v1;

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Inverse");
        }

        [Test]
        public void WHEN_String_Values_Same_Value_SHOULD_Render_Primary_Template()
        {
            //Arrange
            string sameValue = GetRandom.String(32);
            string v1 = sameValue;
            string v2 = sameValue;

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_String_Values_Equivalent_Values_SHOULD_Render_Primary_Template()
        {
            //Arrange
            string sameValue = GetRandom.String(32);
            string v1 = new string(sameValue.ToCharArray());
            string v2 = new string(sameValue.ToCharArray());

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Primary");
        }

        [Test]
        public void WHEN_String_Values_Different_Values_SHOULD_Render_Inverse_Template()
        {
            //Arrange
            string v1 = GetRandom.String(32);
            string v2 = GetRandom.String(32);
            while (v2 == v1) { v2 = GetRandom.String(32); }

            //Act
            var result = _template.Invoke(new { V1 = v1, V2 = v2 });

            //Assert
            result.Should().Be("Inverse");
        }
    }
}
