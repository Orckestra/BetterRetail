using System;
using System.Reflection;
using Fasterflect;

namespace Orckestra.Composer.ViewModels
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormattingAttribute : Attribute
    {
        public string FormatCategory { get; set; }
        public string FormatKey { get; set; }

        public FormattingAttribute(string formatCategory, string formatKey)
        {
            FormatCategory = formatCategory;
            FormatKey = formatKey;
        }
        public static FormattingAttributeValues GetFormattingValues(PropertyInfo propertyInfo)
        {
            if (propertyInfo.HasAttribute<FormattingAttribute>())
            {
                var attr = propertyInfo.Attribute<FormattingAttribute>();
                return new FormattingAttributeValues {Category = attr.FormatCategory, Key = attr.FormatKey};
            }
            return null;
        }
        public static bool IsPropertyFormattable(PropertyInfo propertyInfo)
        {
            return propertyInfo.HasAttribute<FormattingAttribute>();
        }
    }
}