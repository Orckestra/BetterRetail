using System.Globalization;

namespace Orckestra.Composer.Providers.Localization
{

    /// <summary>
    /// Request definition to identity the common resource string.
    /// </summary>
    public class GetLocalizedParam
    {
        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the category where the localized string is located.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the key that refers to the localized string to retrieve for the given category.
        /// </summary>
        public string Key { get; set; }
    }
}
