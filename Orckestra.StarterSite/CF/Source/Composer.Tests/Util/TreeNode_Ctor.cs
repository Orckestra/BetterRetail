using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class TreeNodeCtor
    {
        [Test]
        public void WHEN_Value_Is_Valid_SHOULD_Instantiate()
        {
            // Arrange
            var value = GetRandom.Int();

            // Act
            var node = new TreeNode<int>(value);

            // Assert
            node.Value.Should().Be(value);
            node.HasParent.Should().BeFalse();
            node.Parent.Should().BeNull();
            node.Children.Should().NotBeNull();
            node.Children.Should().BeEmpty();
        }
    }
}
