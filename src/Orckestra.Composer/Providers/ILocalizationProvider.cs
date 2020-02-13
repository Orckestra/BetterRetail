
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.Providers
{
    /// <summary>
    /// Localization provider for common carts strings.
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Gets All the localized Values for All categories of a given culture.
        /// </summary>
        /// <param name="culture">The culture to load</param>
        /// <returns>A tree like structure containing all localized Values per categories</returns>
        Task<LocalizationTree> GetLocalizationTreeAsync(CultureInfo culture);

        /// <summary>
        /// Gets the localized string based on the category, the key and the culture.
        /// </summary>
        /// <param name="param">The parameter which contains the category, the key and the culture.</param>
        /// <returns>
        /// Localized string
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// GetLocalizedParam.Category
        /// or
        /// GetLocalizedParam.Key
        /// or
        /// GetLocalizedParam.Culture
        /// </exception>
        string GetLocalizedString(GetLocalizedParam param);
    }
}
