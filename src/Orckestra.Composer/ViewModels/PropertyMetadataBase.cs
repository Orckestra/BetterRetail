using System;
using System.ComponentModel;
using System.Reflection;
using Fasterflect;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.ViewModels
{
    public class PropertyMetadataBase
    {
        public PropertyMetadataBase(PropertyInfo propertyInfo)
        {
	        if (propertyInfo == null) { throw new ArgumentNullException(nameof(propertyInfo)); }

			var displayNameAttr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>(true);

		    PropertyName = propertyInfo.Name;
	        SourcePropertyName = MapToAttribute.GetMapToPropertyName(propertyInfo);
		    DisplayName = displayNameAttr == null || string.IsNullOrWhiteSpace(displayNameAttr.DisplayName)
			    ? propertyInfo.Name
			    : displayNameAttr.DisplayName;
		    PropertyType = propertyInfo.PropertyType;

            if (propertyInfo.HasAttribute<EnumLocalizationAttribute>())
            {
                LocalizableEnumProperty = true;
                var attributevalues = propertyInfo.Attribute<EnumLocalizationAttribute>();
                if (attributevalues != null)
                {
                    PropertyEnumLocalizationCategory = attributevalues.LocalizationCategory;
                    PropertyEnumAllowEmptyValue = attributevalues.AllowEmptyValue;
                }
            }

            if (FormattingAttribute.IsPropertyFormattable(propertyInfo))
	        {
	            FormattableProperty = true;
	            var attributevalues = FormattingAttribute.GetFormattingValues(propertyInfo);
	            if (attributevalues != null)
	            {
                    PropertyFormattingCategory = attributevalues.Category;
                    PropertyFormattingKey = attributevalues.Key;
	            }
	        }
            SetLookupValues(propertyInfo);
        }
        /// <summary>
        /// Defines the name of the property.
        /// </summary>
        public string PropertyName { get; protected set; }
        //todo: can we convert all protected setters to private?

        /// <summary>
        /// Defines the property to map to in the Overture Dto.
        /// </summary>
        public string SourcePropertyName { get; protected set; }

        public bool FormattableProperty { get; protected set; }

        public bool LocalizableEnumProperty { get; protected set; }

        public string PropertyEnumLocalizationCategory { get; protected set; }

        public bool PropertyEnumAllowEmptyValue { get; protected set; }

        public string PropertyFormattingCategory { get; protected set; }

        public string PropertyFormattingKey { get; protected set; }

        /// <summary>
        /// Defines the display name of the property.
        /// </summary>
        public string DisplayName { get; protected set; }

        public bool LookupProperty { get; protected set; }

        public string LookupName { get; protected set; }

        public LookupType LookupType { get; protected set; }

        public string LookupDelimiter { get; protected set; }

        /// <summary>
        /// Determines the type of the property.
        /// </summary>
        public Type PropertyType { get; protected set; }

        //todo: should it go to class LookupAttribute ?
        protected void SetLookupValues(PropertyInfo propertyInfo)
        {
            LookupProperty = propertyInfo.HasAttribute<LookupAttribute>();
            if (LookupProperty)
            {
                var lookupAttribute = propertyInfo.Attribute<LookupAttribute>();
                LookupName = lookupAttribute.LookupName;
                LookupType = lookupAttribute.LookupType;
                LookupDelimiter = lookupAttribute.Delimiter;
            }
        }
    }
}
