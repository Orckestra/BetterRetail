using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.Providers.Localization
{
    internal interface ILocalizationRepository<TSource>
    {
        /// <summary>
        /// Get all categories used to bundle localization
        /// </summary>
        /// <returns></returns>
        List<string> GetAllCategories();

        /// <summary>
        /// Get all possible sources of localization for a given (key, category, culture).
        /// 
        /// The resulting list of sources is sorted with most significant source first
        /// </summary>
        List<TSource> GetPrioritizedSources(string categoryName, CultureInfo culture);

        /// <summary>
        /// Get all values for all keys in a single source
        /// Sources can be found using <see cref="GetPrioritizedSources"/>
        /// </summary>
        Task<Dictionary<string, string>> GetValuesAsync(TSource source);
    }
}
