using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.IntegrationTests
{
    [TestFixture]
    public class ViewModel_ClassDef
    {
        private const string ComposerDllRegexPattern = "^.*\\\\Orckestra\\.Composer(\\.(.+))?.dll$";
        private readonly Regex _composerDllRegex = new Regex(ComposerDllRegexPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private string _executingPath;

        [SetUp]
        public void SetUp()
        {
            _executingPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);
        }

        private static bool IsViewModel(Type t)
        {
            return 
                (t.BaseType != null && t.BaseType.FullName == typeof(BaseViewModel).FullName)
                || (t.IsClass
                    && t.FullName != typeof(BaseViewModel).FullName
                    && t.Name.EndsWith("ViewModel"));
        }

        private List<Assembly> LoadComposerAssemblies()
        {
            var dllPaths = GetEligibleAssemblyFilePaths();
            var loadedAssemblies = new List<Assembly>();

            foreach (var dllPath in dllPaths)
            {
                var assembly = Assembly.LoadFile(dllPath);
                loadedAssemblies.Add(assembly);
            }

            return loadedAssemblies;
        }

        private IEnumerable<string> GetEligibleAssemblyFilePaths()
        {
            var allDllPaths = Directory.GetFiles(_executingPath, "*.dll");

            var dlls = allDllPaths.Where(IsAssemblyElligible);
            return dlls;
        }

        private bool IsAssemblyElligible(string dllPath)
        {
            return _composerDllRegex.IsMatch(dllPath) && !IsTestDll(dllPath);
        }

        private bool IsTestDll(string dllPath)
        {
            var fileName = Path.GetFileName(dllPath);
            var index = fileName.LastIndexOf("Test", StringComparison.InvariantCultureIgnoreCase);

            return index > 1;
        }
    }
}
