using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Kernel;

namespace Orckestra.Composer.Tests.ComposerHost
{
    [TestFixture]
    public class ComposerAssembliesValidateWeightAttribute
    {
        Dictionary<string, _Assembly> _appDomainAssemblies = new Dictionary<string, _Assembly>();
        private Composer.AssemblyHelper _assemblyHelper;
        private const string ComposerDllRegex = "^.*\\\\Orckestra\\.Composer(\\.(.+))?.dll$";
        private Composer.ComposerHost _composerHost;
        

        [SetUp]
        public void Setup()
        {
            _composerHost = new Composer.ComposerHost();
            _assemblyHelper = new Composer.AssemblyHelper();
            var assemblies = new List<_Assembly>();

            // Load composer assemblies
            var loadedAssemblies = _assemblyHelper.SafeLoadAssemblies(ComposerDllRegex);
            assemblies.AddRange(loadedAssemblies);

            // Load other assemblies next to executing assembly
            assemblies.AddRange(_assemblyHelper.SafeLoadAssemblies());

            // Load specific assemblies
            assemblies.Add(Assembly.GetExecutingAssembly());

            // Add assemblies to dictionary
            foreach (var assembly in assemblies)
            {
                _appDomainAssemblies[assembly.FullName] = assembly;
            }
        }
        [Test]
        public void When_Assembly_Is_Missing_Attribute_SHOULD_be_at_the_end_of_collection()
        {
            // Arrange

            // Act
            var sortedByWeight = _composerHost.SortExtensionAssembliesByWeight(_appDomainAssemblies.Values);
            if (sortedByWeight.Count > 1)
            {
                var lastAssembly = sortedByWeight[sortedByWeight.Count - 1];
                // Assert
                lastAssembly.FullName.Should().Contain("Orckestra.Composer.Tests");
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public void WHEN_assemblies_for_plugin_registration_are_retrieved_they_SHOULD_HAVE_ComposerAssemblyWeightAttribute()
        {
            // Arrange

            // Act
            var sortedassemblies = _composerHost.SortExtensionAssembliesByWeight(_appDomainAssemblies.Values);
 
            //assert
            foreach (var assembly in sortedassemblies)
            {
                var attributes = assembly.GetCustomAttributes(typeof(ComposerAssemblyWeightAttribute), false);

                //skip this one as it has no attribute by purpose
                if (assembly.FullName.Contains("Orckestra.Composer.Tests"))
                    continue;

                //real assert
                Console.WriteLine(assembly.FullName);
                attributes.Length.Should().Be(1, $"Assembly '{assembly.FullName}' should have ComposerAssemblyWeight attribute");
            }
        }
        [Test]
        public void When_Assemblies_are_being_registered_it_SHOULD_be_in_the_right_order()
        {
            // Arrange

            // Act
            var sortedassemblies = _composerHost.SortExtensionAssembliesByWeight(_appDomainAssemblies.Values);
 
            //assert
            int previousWeight = 0;
            foreach (var assembly in sortedassemblies)
            {
                var attributes = assembly.GetCustomAttributes(typeof(ComposerAssemblyWeightAttribute), false);
                if (attributes.Length > 0)
                {
                    if (attributes[0] is ComposerAssemblyWeightAttribute attribute)
                    {
                        int weight = attribute.ComposerAssemblyWeigh;

                        //real assert
                        weight.Should().BeGreaterOrEqualTo(previousWeight);

                        previousWeight = attribute.ComposerAssemblyWeigh;
                    }
                }
            }
        }
        [Test]
        public void WHEN_checking_assembly_info_there_SHOULD_BE_ComposerWeightAttribute()
        {

            //arrange
            var assembly = Assembly.Load("Orckestra.Composer");
            var attributes = assembly.GetCustomAttributes(typeof(ComposerAssemblyWeightAttribute), false);

            //assert
            attributes.Length.Should().Be(1);

        }
    }
}