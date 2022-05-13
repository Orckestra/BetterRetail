using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Orckestra.Composer.TypeExtensions;

namespace Orckestra.Composer
{
    internal static class AssemblyExtensions
    {
        public static bool IgnoreExceptions { get; set; }

        internal static bool ReferencesAssembly(this _Assembly assembly, Assembly referencedAssembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (referencedAssembly == null) throw new ArgumentNullException(nameof(referencedAssembly));

            string targetAssemblyName = referencedAssembly.GetName().Name;

            return assembly.GetName().Name == targetAssemblyName || assembly.GetReferencedAssemblies().Any(r => r.Name == targetAssemblyName);
        }

        public static IEnumerable<Type> SafeConcretTypesOf<TOf>(this Assembly assembly)
        {
            return assembly.SafeGetTypes().Where(t => typeof(TOf).IsAssignableFromConcret(t));
        }

        public static IEnumerable<Type> SafeGetTypes(this _Assembly assembly)
        {
            if (assembly == null) { throw new ArgumentNullException(nameof(assembly)); }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                if (IgnoreExceptions) { return new List<Type>(); }

                var builder = new StringBuilder();

                foreach (var exception in ex.LoaderExceptions)
                {
                    builder.AppendLine(exception.Message);
                }

                throw new InvalidOperationException(builder.ToString());
            }
        }
    }
}