using System;
using System.Collections.Concurrent;

namespace Orckestra.Composer.TypeExtensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, object> DefaultValues = new ConcurrentDictionary<Type, object>(); 

        public static object GetDefaultValue(this Type t)
        {
            if (t == null) { throw new ArgumentNullException(nameof(t)); }

            if (!t.IsValueType) { return null; }

            if (!DefaultValues.ContainsKey(t))
            {
                var defaultValue = Activator.CreateInstance(t);
                DefaultValues.TryAdd(t, defaultValue);
            }

            var isSuccess = DefaultValues.TryGetValue(t, out object value);

            if (!isSuccess) { throw new Exception("Something really wrong has happened with multi-threading..."); }

            return value;
        }

        public static bool IsAssignableFromConcret(this Type type, Type typeFrom)
        {
            if (type == null) { return false; }

            if (typeFrom == null) { throw new ArgumentNullException(nameof(typeFrom)); }

            return type.IsAssignableFrom(typeFrom) && !typeFrom.IsAbstract && !typeFrom.IsInterface;
        }
    }
}