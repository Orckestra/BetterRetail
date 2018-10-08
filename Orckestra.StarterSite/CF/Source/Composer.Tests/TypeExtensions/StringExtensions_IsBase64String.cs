using System;
using System.Text;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;

namespace Orckestra.Composer.Tests.TypeExtensions
{
    [TestFixture]
    public class StringExtensions_IsBase64String
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("PATATE")]
        [TestCase("ABCDEF")]
        [TestCase("  ABCDEF================= ")]
        public void WHEN_string_is_not_base64_SHOULD_return_false(string str)
        {
            //Arrange

            //Act
            var isBase64 = str.IsBase64String();

            //Arrange
            isBase64.Should().BeFalse();
        }

        [TestCase("PATATE")]
        [TestCase("         |       ")]
        [TestCase("ABCDEF")]
        [TestCase("ABCDEF_ABC")]
        [TestCase("Hello world!")]
        [TestCase("Hello world!Hello world!Hello world!Hello world!Hello world!Hello world!Hello world!Hello world!Hello world!Hello world!Hello world!")]
        public void WHEN_string_is_base64_SHOULD_return_true(string str)
        {
            //Arrange
            var base64Str = CreateBase64String(str);

            //Act
            var isBase64 = base64Str.IsBase64String();

            //Assert
            isBase64.Should().BeTrue();
        }

        [Test]
        public void WHEN_string_has_padding_SHOULD_return_true()
        {
            //Arrange
            var str = CreateBase64String(GetRandom.String(187));
            str = "         " + str + "   ";

            //Act
            var isBase64 = str.IsBase64String();

            //Assert
            isBase64.Should().BeTrue();
        }

        private static string CreateBase64String(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var base64Str = Convert.ToBase64String(bytes);
            return base64Str;
        }
    }
}
