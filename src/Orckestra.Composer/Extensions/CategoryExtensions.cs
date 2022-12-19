using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Products;
using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Extensions
{
    /// <summary>
    /// Extends the <see cref="Category" />
    /// </summary>
    public static class CategoryExtensions
    {
        /// <summary>
        /// Build the Category Path for the categoryId
        /// </summary>
        /// <returns>List of Categories</returns>
        public static List<Category> BuildPathFromTree(this Tree<Category, string> categoriesTree, string categoryId)
        {
            if (!categoriesTree.TryGetValue(categoryId, out TreeNode<Category> categoryNode))
            {
                throw new ArgumentException($"Category with Id '{categoryId}' doesn't exist", nameof(categoryId));
            }

            var path = new List<Category>
            {
                categoryNode.Value
            };

            while (categoryNode.HasParent)
            {
                categoryNode = categoryNode.Parent;
                path.Add(categoryNode.Value);
            }

            return path;
        }

    }
}
