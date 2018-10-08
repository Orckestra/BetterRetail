using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Orckestra.Composer.Tests.ComposerHost
{
    [TestFixture(Category = "ComposerHost")]
    public class ComposerHostCtorTests
    {
        [Test]
        public void UsingDefaultCtor_AutoCrawlEnabled_Should_Be_True_And_AssembliesToCrawl_Should_Be_Empty()
        {
            //Arrange

            //Act
            var host = new Composer.ComposerHost();

            //Assert
            host.AutoCrawlEnabled.Should().BeTrue();
            host.AssembliesToInclude.Should().NotBeNull();
            host.AssembliesToInclude.Should().BeEmpty();
        }

        [Test]
        public void UsingParametizedCtor_AutoCrawlEnabled_Should_Be_False_And_AssembliesToCrawl_Should_Be_Equal()
        {
            //Arrange
            var assemblyMock = new Mock<_Assembly>();

            var assemblies = new[]
            {
                GetType().Assembly,
                assemblyMock.Object,
                assemblyMock.Object
            };

            //Act
            var host = new Composer.ComposerHost(assemblies);

            //Assert
            host.AutoCrawlEnabled.Should().BeFalse();
            host.AssembliesToInclude.Should().NotBeNull();

            foreach (var assembly in assemblies)
            {
                host.AssembliesToInclude.Should().Contain(assembly);
            }
        }

        [Test]
        public void UsingParametizedCtor_With_Null_Should_Throw_Argument_Exception()
        {
            // Arrange
            
            // Act
            Action action = () => new Composer.ComposerHost(null);

            // Assert
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void UsingParametizedCtor_With_Empty_Assemblies_Should_Throw_Argument_Exception()
        {
            // Arrange

            // Act
            Action action = () => new Composer.ComposerHost(new _Assembly[0]);

            // Assert
            action.ShouldThrow<ArgumentException>();
        }
    }
}
