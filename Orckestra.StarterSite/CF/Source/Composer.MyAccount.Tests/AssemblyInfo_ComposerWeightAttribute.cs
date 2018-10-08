using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Kernel;

namespace Orckestra.Composer.MyAccount.Tests
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public class AssemblyInfo_ComposerWeightAttribute
    {
        [Test]
        public void WHEN_checking_assembly_info_there_SHOULD_BE_ComposerWeightAttribute()
        {
            
            //arrange
            var assembly = Assembly.Load("Orckestra.Composer.MyAccount");
            var attributes = assembly.GetCustomAttributes(typeof(ComposerAssemblyWeightAttribute), false);

            //assert
            attributes.Length.Should().Be(1);
            
        }
    }
}