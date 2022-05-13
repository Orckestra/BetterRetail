using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;

namespace Orckestra.Composer
{
    internal class AssemblyHelper
    {
        internal string GetExecutingAssemblyPath()
        {
            var path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            return path;
        }

        internal IEnumerable<string> GetAssemblyFiles()
        {
            return Directory.GetFiles(GetExecutingAssemblyPath() ?? string.Empty, "*.dll");
        }

        /// <summary>
        /// Loads all assemblies found in the executing bin folder.
        /// </summary>
        /// <returns>Assemblies that were loaded successfully.</returns>
        public IEnumerable<_Assembly> SafeLoadAssemblies()
        {
            return LoadAssemblies(GetAssemblyFiles());
        }

        /// <summary>
        /// Loads assemblies found in the executing bin folder if they start with the specified name.
        /// </summary>
        /// <param name="pattern">Regular expression pattern the dll name must match to be loaded.</param>
        /// <returns>Assemblies that were loaded successfully.</returns>
        public virtual IEnumerable<_Assembly> SafeLoadAssemblies(string pattern)
        {
            if (pattern == null) { throw new ArgumentNullException(nameof(pattern)); }

            var assemblyFiles = GetAssemblyFiles().Where(af => Regex.IsMatch(Path.GetFileName(af), pattern));

            return LoadAssemblies(assemblyFiles);
        }

        /// <summary>
        /// Loads the specified assemblies in the AppDomain.
        /// </summary>
        /// <param name="assemblyFileNames">Assembly files to load. May not be null.</param>
        /// <returns>Assemblies that were loaded successfully.</returns>
        private IEnumerable<_Assembly> LoadAssemblies(IEnumerable<string> assemblyFileNames)
        {
            if (assemblyFileNames == null) { throw new ArgumentNullException(nameof(assemblyFileNames)); }

            var loadedAssemblies = new List<_Assembly>();
            foreach (var assemblyFile in assemblyFileNames)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyFile);
                    loadedAssemblies.Add(assembly);
                }
                catch (Win32Exception) 
                {
                    //Explictly left empty
                }
                catch (ArgumentException) 
                {
                    //Explictly left empty
                }
                catch (FileNotFoundException) 
                {
                    //Explictly left empty
                }
                catch (PathTooLongException)
                {
                    //Explictly left empty
                }
                catch (BadImageFormatException)
                {
                    //Explictly left empty
                }
                catch (SecurityException)
                {
                    //Explictly left empty
                }
            }
            return loadedAssemblies;
        }
    }
}