using System;

namespace Orckestra.Composer.Kernel
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ComposerAssemblyIgnoreAttribute : Attribute
    {
        public ComposerAssemblyIgnoreAttribute()
        {
        }
    }
}