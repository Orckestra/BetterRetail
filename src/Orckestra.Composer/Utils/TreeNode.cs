using System.Collections.Generic;

namespace Orckestra.Composer.Utils
{
    public class TreeNode<T>
    {
        public TreeNode<T> Parent { get; set; }
        public List<TreeNode<T>> Children { get; set; }

        public T Value { get; private set; }

        public bool HasParent
        {
            get { return Parent != null; }
        }

        public TreeNode(T value)
        {
            Value = value;
            Children = new List<TreeNode<T>>();
        }

        /// <summary>
        /// Gets the level at which this node is at.
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            int level = 0;
            var parentNode = Parent;

            while (parentNode != null)
            {
                level++;
                parentNode = parentNode.Parent;
            }

            return level;
        }
    }
}
