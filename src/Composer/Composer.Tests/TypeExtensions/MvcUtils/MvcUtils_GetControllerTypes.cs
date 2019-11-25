using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.ForTests;

namespace Orckestra.Composer.Tests.TypeExtensions.MvcUtils
{
    [TestFixture]
    public class MvcUtilsGetControllerTypes : BaseTestForStaticSut
    {
        [Test]
        public void WHEN_assemblyToCrawl_Is_Null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            _Assembly nullAssembly = null;

            //Act
            var action = new Action(() => Composer.TypeExtensions.MvcUtils.GetControllerTypes(nullAssembly));

            //Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_assemblyToCrawl_contains_no_valid_controller_SHOULD_return_empty_enumerable()
        {
            //Arrange
            var types = new Type[]
            {
                typeof (TestNotWorkingController),
                typeof (FakeClass),
                typeof (NotRespectingNomenclatureCont),
                typeof(InternalController),
                typeof(AbstractController)
            };

            Mock<_Assembly> assemblyMock = new Mock<_Assembly>();
            assemblyMock
                .Setup(a => a.GetTypes())
                .Returns(types)
                .Verifiable();

            //Act
            var controllers = Composer.TypeExtensions.MvcUtils.GetControllerTypes(assemblyMock.Object);

            //Assert
            controllers.Should().BeEmpty();
            new Action(() => assemblyMock.Verify()).ShouldNotThrow();
        }

        [Test]
        public void WHEN_assemblyToCrawl_contains_valid_controllers_SHOULD_return_enumerable_of_types()
        {
            //Arrange
            var types = new Type[]
            {
                typeof (TestNotWorkingController),
                typeof (FakeClass),
                typeof (NotRespectingNomenclatureCont),
                typeof (InternalController),
                typeof (AbstractController),
                typeof (ValidController)
            };

            Mock<_Assembly> assemblyMock = new Mock<_Assembly>();
            assemblyMock
                .Setup(a => a.GetTypes())
                .Returns(types)
                .Verifiable();

            //Act
            var controllerTypes = Composer.TypeExtensions.MvcUtils.GetControllerTypes(assemblyMock.Object);

            //Assert
            controllerTypes.Should().NotBeEmpty();
            controllerTypes.Should().Contain(typeof(ValidController));
        }

    }
}
