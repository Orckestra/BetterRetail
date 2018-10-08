using System;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class TreeCtor
    {
        [Test]
        public void WHEN_Arguments_Are_Valid_SHOULD_Instantiate()
        {
            // Arrange
            var objects = new[]
            {
                new NodeObject { Key = "A", ParentKey = null },
                new NodeObject { Key = "B", ParentKey = "A" },
                new NodeObject { Key = "C", ParentKey = "A" }
            };

            Func<NodeObject, string> keySelector = (obj) => obj.Key;
            Func<NodeObject, string> parentKeySelector = (obj) => obj.ParentKey;

            // Act
            var tree = new Tree<NodeObject, string>(objects, keySelector, parentKeySelector);

            // Assert
            tree.Should().HaveCount(objects.Length);
            tree.Should().ContainKey("A");
            tree.Should().ContainKey("B");
            tree.Should().ContainKey("C");

            // Test node with key "A"
            var node = tree["A"];
            node.HasParent.Should().BeFalse();
            node.Children.Should().HaveCount(2);

            var nodeValue = node.Value;
            nodeValue.Key.Should().Be("A");
            nodeValue.ParentKey.Should().BeNull();

            // Test node with key "B"
            node = tree["B"];
            node.HasParent.Should().BeTrue();
            node.Parent.Value.Key.Should().Be("A");
            node.Children.Should().HaveCount(0);

            nodeValue = node.Value;
            nodeValue.Key.Should().Be("B");
            nodeValue.ParentKey.Should().Be("A");
        }

        class NodeObject
        {
            public string Key { get; set; }
            public string ParentKey { get; set; }
        }
    }
}
