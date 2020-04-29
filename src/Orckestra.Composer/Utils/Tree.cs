using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Utils
{
    public class Tree<TValue, TKey> : Dictionary<TKey, TreeNode<TValue>>
    {
        public Tree(IEnumerable<TValue> source, Func<TValue, TKey> keySelector, Func<TValue, TKey> parentKeySelector)
        {
            Init(source, keySelector, parentKeySelector);
        }

        public Tree(IEnumerable<TValue> source, Func<TValue, TKey> keySelector, Func<TValue, TKey> parentKeySelector,
            IEqualityComparer<TKey> comparer)
            :base(comparer)
        {
            Init(source, keySelector, parentKeySelector);
        }

        private void Init(IEnumerable<TValue> source, Func<TValue, TKey> keySelector,
            Func<TValue, TKey> parentKeySelector)
        {
            // Feed current dictionary
            foreach (var item in source)
            {
                this[keySelector(item)] = new TreeNode<TValue>(item);
            }

            // Link TreeNodes
            foreach (var kvp in this)
            {
                var category = kvp.Value;

                var parentKey = parentKeySelector(category.Value);
                if (parentKey != null
                    && TryGetValue(parentKey, out TreeNode<TValue> parent))
                {
                    category.Parent = parent;
                    parent.Children.Add(category);
                }
            }
        }
    }
}
