using System;
using System.Collections;
using System.Globalization;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.Search.Providers
{
    public class FacetLocalizationProvider : IFacetLocalizationProvider
    {
        public FacetLocalizationProvider(ILocalizationProvider localizationProvider)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        private ILocalizationProvider LocalizationProvider { get; set; }

        /// <summary>
        ///     Convert the Facet field name into a Displayable Title.
        ///     The base implementation will look into the LocalizationProvider for a Key that match the format
        ///     {FieldName}_Title
        /// </summary>
        /// <param name="fieldName">FieldName of the Facet as returned by the Search Provider</param>
        /// <param name="cultureInfo">Culture</param>
        /// <returns>The localized field name if localized; the initial value otherwise</returns>
        public string GetFormattedFacetTitle(string fieldName, CultureInfo cultureInfo)
        {
            var localizedValue = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = SearchConfiguration.FormatFacetLocalizationCategory,
                Key = SearchConfiguration.FormatFacetTitleLocalizationKeyPattern.Replace("{FieldName}", fieldName),
                CultureInfo = cultureInfo
            });

            return localizedValue ?? string.Format("[{0}]", fieldName);
        }

        /// <summary>
        ///     Convert the Facet value into a Displayable Title.
        ///     The base implementation will look into the LocalizationProvider for a Key that match the format
        ///     {FieldName}_{Value}
        ///     For example, if you have the Facet "Availablility" with values "true" and "false"
        ///     You can add the following Localization Keys in the FacetFormatting Category
        ///     Availability_true => "Disponible"
        ///     Availability_false => "Non Disponible"
        /// </summary>
        /// <param name="fieldName">FieldName of the Facet as returned by the Search Provider</param>
        /// <param name="value">Value to be formatted, case sensitive</param>
        /// <param name="cultureInfo">Culture</param>
        /// <returns>The localized value if localized; the initial value otherwise</returns>
        public string GetFormattedFacetValueTitle(string fieldName, string value, CultureInfo cultureInfo)
        {
            var localizedValue = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = SearchConfiguration.FormatFacetLocalizationCategory,
                Key = SearchConfiguration.FormatFacetLocalizationKeyPattern.Replace("{FieldName}", fieldName)
                    .Replace("{Value}", value),
                CultureInfo = cultureInfo
            });

            return localizedValue ?? value;
        }

        /// <summary>
        ///     Format the values of a range facet into a displayable text.
        ///     The base implementation will look into the LocalizationProvider for a Key that match the format:
        ///     If only <paramref name="minValue" /> is defined: {FieldName}_MinValuePattern
        ///     If only <paramref name="maxValue" /> is defined: {FieldName}_MaxValuePattern
        ///     If both <paramref name="minValue" /> and <paramref name="maxValue" /> are defined: {FieldName}_MinMaxValuePattern
        /// </summary>
        /// <param name="fieldName">FieldName of the Facet as returned by the Search Provider</param>
        /// <param name="minValue">Minimum value of the range facet</param>
        /// <param name="maxValue">Maximum value of the range facet</param>
        /// <param name="cultureInfo">Culture</param>
        /// <param name="valueType">Type of the min and max values.</param>
        /// <returns>The localized values if localized; the initial value otherwise</returns>
        public string GetFormattedRangeFacetValues(string fieldName, string minValue, string maxValue, Type valueType, CultureInfo cultureInfo)
        {
            string formatKeyPattern;
            var formatParams = new ArrayList();

            if (string.IsNullOrWhiteSpace(minValue))
            {
                if (string.IsNullOrWhiteSpace(maxValue))
                {
                    throw new ArgumentException("At least one of the parameters minValue or maxValue must be set");
                }
                formatKeyPattern = SearchConfiguration.FormatRangeFacetMaxValueLocalizationKeyPattern;
                formatParams.Add(Convert.ChangeType(maxValue, valueType));
            }
            else
            {
                formatParams.Add(Convert.ChangeType(minValue, valueType));
                if (string.IsNullOrWhiteSpace(maxValue))
                {
                    formatKeyPattern = SearchConfiguration.FormatRangeFacetMinValueLocalizationKeyPattern;
                }
                else
                {
                    formatKeyPattern = SearchConfiguration.FormatRangeFacetMinMaxValueLocalizationKeyPattern;
                    formatParams.Add(Convert.ChangeType(maxValue, valueType));
                }
            }

            var localizedFormat = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = SearchConfiguration.FormatFacetLocalizationCategory,
                Key = formatKeyPattern.Replace("{FieldName}", fieldName),
                CultureInfo = cultureInfo
            });

            if (localizedFormat != null)
            {
                try
                {
                    return string.Format(localizedFormat, formatParams.ToArray());
                }
                catch (FormatException)
                {
                    // TODO: Log here
                    throw;
                }
            }

            return string.Format("[{0}]", string.Join(" - ", formatParams.ToArray()));
        }
    }
}