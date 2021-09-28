using System.Globalization;

namespace Orckestra.Composer.Services
{
    public interface ICultureService
    {
        /// <summary>
        /// Gets the default culture of the application.
        /// </summary>
        /// <returns></returns>
        CultureInfo GetDefaultCulture();

        /// <summary>
        /// Gets all supported cultures by the application.
        /// </summary>
        /// <returns></returns>
        CultureInfo[] GetAllSupportedCultures();

        /// <summary>
        /// Gets all supported cultures except the current culture.
        /// </summary>
        /// <param name="skipCulture">Specifies the current culture.</param>
        /// <returns></returns>
        CultureInfo[] GetSupportedCulturesExcept(CultureInfo skipCulture);

        /// <summary>
        /// Checks if a culture is supported by the application or not.
        /// </summary>
        /// <param name="cultureToCheck"></param>
        /// <returns></returns>
        bool IsCultureSupported(CultureInfo cultureToCheck);

        /// <summary>
        /// Gets first affinity culture by the application.
        /// </summary>
        /// <param name="cultureToCheck"></param>
        /// <returns></returns>
        CultureInfo GetAffinityCulture(CultureInfo cultureToCheck);
    }
}
