using System;

namespace Orckestra.Composer.ViewModels
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class MetadataIgnoreAttribute : Attribute
    {
    }
}