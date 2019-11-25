using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Providers
{
    public interface IAutocompleteProvider
    {
        /// <summary>
        /// Looks for the provided keyword in a list of possible suggested terms
        /// </summary>
        /// <param name="keyword">Search keyword to look for a redirect for</param>
        /// <returns></returns>
        Task<List<string>> GetSearchSuggestedTerms(CultureInfo cultureInfo, string keyword);
    }
}