using System;
using System.Globalization;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.ViewModels
{
    public class ResxViewModelPropertyFormatter : IViewModelPropertyFormatter
    {
        private readonly ILocalizationProvider _localizationProvider;

        public ResxViewModelPropertyFormatter(ILocalizationProvider localizationProvider)
        {
            _localizationProvider = localizationProvider;
        }

        public string Format(object value, IPropertyMetadata propertyMetadata, CultureInfo cultureInfo)
        {
            if (propertyMetadata == null) { throw new ArgumentNullException(nameof(propertyMetadata)); }
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            if (value == null) { return null; }

            var localFormattingString = _localizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category    = propertyMetadata.PropertyFormattingCategory,
                Key         = propertyMetadata.PropertyFormattingKey,
                CultureInfo = cultureInfo
            });

            return localFormattingString == null
                ? value.ToString()
                : string.Format(cultureInfo, localFormattingString, value);
        }
    }
}
