using System;
using System.Globalization;
using System.Threading;

namespace Orckestra.Composer.Providers.Localization
{
    // TODO: This helper shoud be removed. (Only in MVC. As of 02/06/15 we can't remove it yet)
    /// <summary>
    /// Method extension used by mvc views to retrieve strings from localization providers.
    /// </summary>
    public static class LocalizationHelper
    {
       /// <summary>
        /// Localizes the strings from views using specified key.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Localize(string category, string key, CultureInfo cultureInfo)
        {
            var localizationProvider = ComposerHost.Current.Resolve<ILocalizationProvider>();

            return localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = category,
                    Key         = key,
                    CultureInfo = cultureInfo
                }
            );
        }

        /// <summary>
        /// Localizeds the format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="toBeFormatted">To be formatted.</param>
        /// <returns></returns>
        public static string LocalizedFormat<T>(string category, string key, T toBeFormatted, CultureInfo cultureInfo)
        {
            var formatter = Localize(category, key, cultureInfo);

            //avoiding the "[error]" when using it in a C1 function/hbs
            if (formatter == null)
            {
                var localizeValue = $"{category}/{key}";
                formatter = "[{0}, " + localizeValue + "]";
            }

            //avoiding the "[error]" when using it in a C1 function/hbs
            if (formatter == null)
            {
                var localizeValue = $"{category}/{key}";
                formatter = "[{0}, " + localizeValue + "]";
            }

            return string.Format(formatter, toBeFormatted);
        }

        [Obsolete("Use Localize passing the cultureInfo as parameter. This function uses the Thread.CurrentThread.CurrentCulture which often don't have the culture used by the user")]
        public static string Localize(string category, string key)
        {
            return Localize(category, key, Thread.CurrentThread.CurrentCulture);
        }

        [Obsolete("Use LocalizedFormat passing the cultureInfo as parameter. This function uses the Thread.CurrentThread.CurrentCulture which often don't have the culture used by the user")]
        public static string LocalizedFormat<T>(string category, string key, T toBeFormatted)
        {
            return LocalizedFormat<T>(category, key, toBeFormatted, Thread.CurrentThread.CurrentCulture);
        }
    }
}
