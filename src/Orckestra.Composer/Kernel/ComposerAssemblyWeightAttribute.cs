using System;

namespace Orckestra.Composer.Kernel
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ComposerAssemblyWeightAttribute : Attribute
    {
        public ComposerAssemblyWeightAttribute(int assemblyWeight)
        {
            ComposerAssemblyWeigh = assemblyWeight;
        }

        public int ComposerAssemblyWeigh { get; private set; }
    }
}