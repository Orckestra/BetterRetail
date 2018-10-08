using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class TreeNodeGetLevel
    {
        public const string RootNodeId = "root";
        public const string Level1NodeId = "level1";
        public const string Level2NodeId = "level2";
        public const string Level3NodeId = "level3";

        [TestCase(RootNodeId, 0)]
        [TestCase(Level1NodeId, 1)]
        [TestCase(Level2NodeId, 2)]
        [TestCase(Level3NodeId, 3)]
        public void WHEN_node_is_at_specified_level_SHOULD_return_specified_level(string nodeId, int expectedLevel)
        {
            //Arrange
            var tree = BuildTree();
            
            //Act
            var node = tree[RootNodeId];
            int? level = node != null ? node.GetLevel() : (int?) null;

            //Assert
            level.Should().NotBe(null, "this means the node was never found");
            level.Should().Be(0, "this is the level {0} item", expectedLevel);
        }

        private Tree<Category, string> BuildTree()
        {
            var testCategories = new List<Category>()
            {
                new Category()
                {
                    Id = RootNodeId,
                    PrimaryParentCategoryId = null
                },
                new Category()
                {
                    Id = Level1NodeId,
                    PrimaryParentCategoryId = RootNodeId
                },
                new Category()
                {
                    Id = Level2NodeId,
                    PrimaryParentCategoryId = Level1NodeId
                },
                new Category()
                {
                    Id = Level3NodeId,
                    PrimaryParentCategoryId = Level2NodeId
                }
            };

            return new Tree<Category, string>(testCategories, c => c.Id, c => c.PrimaryParentCategoryId);
        } 
    }
}
