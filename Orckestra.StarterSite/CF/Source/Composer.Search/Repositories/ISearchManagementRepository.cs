using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Repositories
{
    /// <summary>
    /// Interface for the search management repository. Where keyword redirect are managed.
    /// </summary>
    public interface ISearchManagementRepository
	{

        /// <summary>
        /// Determines the target page to redirect to if a keyword matches
        /// </summary>
        /// <param name="keyword">Search keyword to look for a redirect for</param>
        /// <returns></returns>
        Task<string> GetSearchKeywordRedirect(string scope, CultureInfo cultureInfo, string keyword);

		/// <summary>
		/// Looks for the provided keyword in a list of possible suggested terms
		/// </summary>
		/// <param name="keyword">Search keyword to look for a redirect for</param>
		/// <returns></returns>
		Task<List<string>> GetSearchSuggestedTerms(string scope, CultureInfo cultureInfo, string keyword);
	}
}
