using System;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.TypeExtensions.MvcUtils
{
    [TestFixture]
    public class MvcUtilsGetActionInfos : BaseTestForStaticSut
    {
        [Test]
        public void WHEN_null_controllerType_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            Type controllerType = null;

            //Act
            var action = new Action(() => Composer.TypeExtensions.MvcUtils.GetActionInfos(controllerType));

            //Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [TestCase(typeof(EmptyController))]
        [TestCase(typeof(OnlyInvalidActionsController))]
        public void WHEN_only_invalid_actions_in_controller_type_SHOULD_return_empty_enumerable(Type controllerType)
        {
            //Arrange

            //Act
            var actionInfos = Composer.TypeExtensions.MvcUtils.GetActionInfos(controllerType);

            //Assert
            actionInfos.Should().BeEmpty();
        }

        [Test]
        public void WHEN_valid_actions_in_controller_type_SHOULD_return_enumerable_with_MethodInfo()
        {
            //Arrange
            var controllerType = typeof (ValidController);

            //Act
            var actionInfos = Composer.TypeExtensions.MvcUtils.GetActionInfos(controllerType);

            //Assert
            actionInfos.Should().NotBeEmpty();
            actionInfos.Should().HaveCount(2);
            actionInfos.Should().Contain(info => info.Name == "Index" || info.Name == "ComplexAction");
        }
    }
}
