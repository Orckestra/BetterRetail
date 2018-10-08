using FluentAssertions;
using NUnit.Framework;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.TypeExtensions.MvcUtils
{
    [TestFixture]
    public class MvcUtilsExtractControllerName : BaseTestForStaticSut
    {
        [TestCase("InvalidName")]
        [TestCase("AnotherValue")]
        [TestCase("InvalidControllerTest")]
        public void WHEN_typeName_not_respecting_naming_convention_SHOULD_return_typeName(string invalidTypeName)
        {
            //Arrange

            //Act
            var controllerName = Composer.TypeExtensions.MvcUtils.ExtractControllerName(invalidTypeName);

            //Assert
            controllerName.ShouldBeEquivalentTo(invalidTypeName);
        }

        [TestCase("TestController", "Test")]
        [TestCase("AnotherController", "Another")]
        public void WHEN_typeName_respects_controller_naming_convention_SHOULD_return_returnName(
            string typeName, string returnName)
        {
            //Arrange

            //Act
            var controllerName = Composer.TypeExtensions.MvcUtils.ExtractControllerName(typeName);

            //Assert
            controllerName.ShouldBeEquivalentTo(returnName);
        }
    }
}
