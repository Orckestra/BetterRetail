using System;
using System.Globalization;

namespace Orckestra.Composer.Search.Providers
{
    public interface IFacetLocalizationProvider
    {
        /// <summary>
        ///     Convert the Facet field name into a Displayable Title.
        ///     The base implementation will look into the LocalizationProvider for a Key that match the format
        ///     {FieldName}_Title
        /// </summary>
        /// <param name="fieldName">FieldName of the Facet as returned by the Search Provider</param>
        /// <param name="cultureInfo">Culture</param>
        /// <returns>The localized field name if localized; the initial value otherwise</returns>
        string GetFormattedFacetTitle(string fieldName, CultureInfo cultureInfo);

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
        string GetFormattedFacetValueTitle(string fieldName, string value, CultureInfo cultureInfo);

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
        string GetFormattedRangeFacetValues(string fieldName, string minValue, string maxValue, Type valueType,
            CultureInfo cultureInfo);
    }
}