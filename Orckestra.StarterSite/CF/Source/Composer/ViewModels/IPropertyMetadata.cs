using System;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.ViewModels
{
    public interface IPropertyMetadata
    {
        /// <summary>
        /// Defines the name of the property.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Defines the property to map to in the Overture Dto.
        /// </summary>
        string SourcePropertyName { get; }

        /// <summary>
        /// Defines the display name of the property.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Determines the type of the property.
        /// </summary>
        Type PropertyType { get; }

        string PropertyEnumLocalizationCategory { get; }

        bool PropertyEnumAllowEmptyValue{ get; }

        bool LocalizableEnumProperty { get; }
        string PropertyFormattingCategory { get; }
        string PropertyFormattingKey { get; }
        bool FormattableProperty { get; }

        bool LookupProperty { get; }
        LookupType LookupType { get; }
        string LookupName { get; }
        string LookupDelimiter { get; }

        /// <summary>
        /// Gets the value of the property. Null if the property cannot be read.
        /// </summary>
        /// <param name="viewModel">Instance of <see cref="BaseViewModel"/> from which to read data from.</param>
        /// <returns>Instance of the value.</returns>
        object GetValue(BaseViewModel viewModel);

        /// <summary>
        /// Sets the value of the property. Does nothing if the property is read-only.
        /// </summary>
        /// <param name="viewModel">Instance of <see cref="BaseViewModel"/> into which the data should be set.</param>
        /// <param name="value">Value to set in the instance's property.</param>
        void SetValue(BaseViewModel viewModel, object value);
    }
}