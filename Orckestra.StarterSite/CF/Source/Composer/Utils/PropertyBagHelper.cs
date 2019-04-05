using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Utils
{
    public static class PropertyBagHelper
    {
        public static T GetValueOrDefault<T>(this Dictionary<string, object> bag, string key)
        {
            return GetValueOrDefault<T>(bag, key, default(T));
        }

        public static T GetValueOrDefault<T>(this Dictionary<string, object> bag, string key, T defaultValue)
        {
            T value = defaultValue;
            if (bag != null && !string.IsNullOrWhiteSpace(key))
            {
                if (bag.ContainsKey(key))
                {
                    var obj = bag[key];
                    try
                    {
                        value = (T)obj;
                    }
                    catch { }
                }
            }
            return value;
        }
    }
}
