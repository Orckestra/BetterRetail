using System;

namespace Orckestra.Composer
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class IgnoreCheckAttribute : Attribute
    {
    }
}
