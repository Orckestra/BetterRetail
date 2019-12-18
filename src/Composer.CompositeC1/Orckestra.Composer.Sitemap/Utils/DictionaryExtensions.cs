using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Utils
{
    internal static class DictionaryExtensions
    {
        public static void AddToList<TK, TV>(this Dictionary<TK, List<TV>> dictionary, TK key, TV value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = new List<TV>();
            }
            dictionary[key].Add(value);
        }
    }
}
