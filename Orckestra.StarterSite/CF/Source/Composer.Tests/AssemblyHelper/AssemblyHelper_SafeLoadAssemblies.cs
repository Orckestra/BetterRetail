using System;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Orckestra.Composer.Tests.AssemblyHelper
{
    [TestFixture]
    public class AssemblyHelperSafeLoadAssemblies
    {
        [Test]
        public void WHEN_Everything_Is_Good_SHOULD_Load_Successfully()
        {
            // Arrange
            var assemblyHelper = new Composer.AssemblyHelper();

            // Act
            var assemblies = assemblyHelper.SafeLoadAssemblies();

            // Assert
            assemblies.Should().NotBeEmpty();
            assemblies.Should().Contain(GetType().Assembly);
        }

        [Test]
        public void WHEN_Pattern_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange
            var assemblyHelper = new Composer.AssemblyHelper();

            // Act
            Action action = () => assemblyHelper.SafeLoadAssemblies(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Pattern_Is_Valid_SHOULD_Load_Successfully()
        {
            // Arrange
            var assemblyHelper = new Composer.AssemblyHelper();

            // Act
            var assemblies = assemblyHelper.SafeLoadAssemblies("Orckestra.*Tests");

            // Assert
            assemblies.Should().NotBeEmpty();
            assemblies.Should().ContainItemsAssignableTo<Assembly>();
            assemblies.Should()
                .OnlyContain(
                    a =>
                        ((Assembly) a).ManifestModule.Name.StartsWith("Orckestra.") &&
                        ((Assembly) a).ManifestModule.Name.EndsWith("Tests.dll"));
        }
    }
}
