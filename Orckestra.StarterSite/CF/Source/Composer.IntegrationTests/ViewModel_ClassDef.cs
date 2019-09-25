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
        private string _targetConfiguration;
        private string _packagingPath;

        [SetUp]
        public void SetUp()
        {
            _executingPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);
            _targetConfiguration = _executingPath.LastIndexOf("Debug", StringComparison.InvariantCultureIgnoreCase) > -1
                ? "Debug"
                : "Release";
            _packagingPath = Path.Combine(_executingPath, "..\\..\\..", "Packaging\\bin\\", _targetConfiguration);

            CopyPackages(Directory.GetFiles(_packagingPath));
        }

        private string GetFinalExecutingDllPath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var newPath = Path.Combine(_executingPath, fileName);

            return newPath;
        }

        private void CopyPackages(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                var newPath = GetFinalExecutingDllPath(filePath);

                try
                {
                    File.Copy(filePath, newPath, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to copy '{0}'. This may be critical...{1}{1}{2}", Path.GetFileName(filePath), Environment.NewLine, ex);
                }
            }
        }

        [Test]
        public void WHEN_class_is_ViewModel_SHOULD_extend_BaseViewModel()
        {
            var assembliesToTest = LoadComposerAssemblies();
            var invalidVmCount = 0;
            var baseType = typeof (BaseViewModel);

            foreach (var assembly in assembliesToTest)
            {
                var viewModelTypes = assembly.GetTypes().Where(IsViewModel);

                foreach (var vmType in viewModelTypes)
                {
                    var def = new ViewModelDefinition(vmType)
                    {
                        InheritsBase = vmType.BaseType.FullName == baseType.FullName,
                        IsSealed = vmType.IsSealed
                    };

                    if (!def.IsValid())
                    {
                        Console.WriteLine(def);
                        invalidVmCount++;
                    }

                }
            }

            Assert.IsNotEmpty(assembliesToTest, $"No assembly found in '${_executingPath}'");

            Console.WriteLine("Found {0} invalid ViewModels.", invalidVmCount);
            Assert.That(invalidVmCount == 0, "Some ViewModels are invalid. Please check output.");
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
