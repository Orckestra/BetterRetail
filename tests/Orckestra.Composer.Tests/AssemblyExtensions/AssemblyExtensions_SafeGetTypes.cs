using System;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Orckestra.Composer.Tests.AssemblyExtensions
{
    [TestFixture]
    public class AssemblyExtensionsSafeGetTypes
    {
        [Test]
        public void WHEN_Assembly_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            // Arrange

            // Act
            Action action = () => Composer.AssemblyExtensions.SafeGetTypes(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Assembly_Is_Valid_SHOULD_Return_Types()
        {
            // Arrange

            // Act
            var result = Composer.AssemblyExtensions.SafeGetTypes(Assembly.GetExecutingAssembly());

            // Assert
            result.Should().NotBeEmpty();
        }
    }
}
