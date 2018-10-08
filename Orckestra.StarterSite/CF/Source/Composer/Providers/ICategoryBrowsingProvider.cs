using System.Collections.Generic;
using System.Collections.Specialized;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Providers
{
	public interface ICategoryBrowsingUrlProvider
	{
		/// <summary>
		/// Builds the search URL
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">The base search URL is null or empty. Unable to build the search URL.</exception>
		string BuildCategoryBrowsingUrl(BuildCategoryBrowsingUrlParam param);

		/// <summary>
		/// Builds the selected facets.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <returns></returns>
		IEnumerable<SearchFilter> BuildSelectedFacets(NameValueCollection queryString);

	    NameValueCollection BuildSearchQueryString(BuildSearchUrlParam param);

	}
}
