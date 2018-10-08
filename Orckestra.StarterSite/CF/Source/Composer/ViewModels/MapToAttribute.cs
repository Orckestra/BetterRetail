using System;
using System.Reflection;
using Fasterflect;

namespace Orckestra.Composer.ViewModels
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MapToAttribute : Attribute
    {
        public string PropertyToMapTo { get; private set; }

        public MapToAttribute(string propertyToMapTo)
        {
            PropertyToMapTo = propertyToMapTo;
        }

        public static string GetMapToPropertyName(PropertyInfo propertyInfo)
        {
            if (propertyInfo.HasAttribute<MapToAttribute>())
            {
                var attr = propertyInfo.Attribute<MapToAttribute>();
                return attr.PropertyToMapTo;
            }

            return propertyInfo.Name;
        }
    }
}