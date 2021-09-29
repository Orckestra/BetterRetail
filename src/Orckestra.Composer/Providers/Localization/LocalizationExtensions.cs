using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

        private static readonly Dictionary<string, CultureInfo> ISOCurrenciesToACultureMap =
            CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(c => new { c, new RegionInfo(c.LCID).ISOCurrencySymbol })
                .GroupBy(x => x.ISOCurrencySymbol)
                .ToDictionary(g => g.Key, g => g.First().c, StringComparer.OrdinalIgnoreCase);

        public static CultureInfo GetCultureByCurrencyIso(this ILocalizationProvider localizationProvider, string currencyCode)
        {
            ISOCurrenciesToACultureMap.TryGetValue(currencyCode, out CultureInfo cultureInfo);
            return cultureInfo;
        }

        public static string FormatPrice(this ILocalizationProvider localizationProvider, decimal price, string currencyCode)
        {
            if (localizationProvider == null) { throw new ArgumentNullException(nameof(localizationProvider)); }
            if (currencyCode == null) { throw new ArgumentNullException(nameof(currencyCode)); }

            ISOCurrenciesToACultureMap.TryGetValue(currencyCode, out CultureInfo cultureInfo);

            return localizationProvider.FormatPrice(price, cultureInfo);
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