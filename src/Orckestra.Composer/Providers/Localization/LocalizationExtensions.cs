using System;
using System.Globalization;

namespace Orckestra.Composer.Providers.Localization
{
    /// <summary>
    /// Localization helper extension.  This object is used to transform prices (decimal) into formated currency depending of the culture.
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Gets a localized error message based on the code of that error which will be used has a key.
        /// </summary>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <param name="errorCode">The code of the error message.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns></returns>
        public static string GetLocalizedErrorMessage(this ILocalizationProvider localizationProvider, string errorCode, CultureInfo cultureInfo)
        {
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (errorCode == null) { throw new ArgumentNullException(nameof(errorCode)); }
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            return localizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "Errors",
                Key = "L_" + errorCode,
                CultureInfo = cultureInfo
            });
        }

        /// <summary>
        /// Formats the price of a product.
        /// </summary>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <param name="price">The price.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns></returns>
        public static string FormatPrice(this ILocalizationProvider localizationProvider, decimal price, CultureInfo cultureInfo)
        {
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            var format = localizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category    = "General",
                Key         = "PriceFormat",
                CultureInfo = cultureInfo
            });

            return string.Format(cultureInfo, format, price);
        }

        public static string FormatPhoneNumber(this ILocalizationProvider localizationProvider, string phoneNumber, CultureInfo cultureInfo)
        {
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (cultureInfo == null) { throw new ArgumentNullException(nameof(cultureInfo)); }

            var format = localizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "Store",
                Key = "PhoneNumberFormat",
                CultureInfo = cultureInfo
            });

            return !string.IsNullOrWhiteSpace(phoneNumber) && double.TryParse(phoneNumber, out double phoneNumberDouble)
                ? string.Format(cultureInfo, format, phoneNumberDouble)
                : phoneNumber;
        }
    }
}