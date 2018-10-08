using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Parameters;

namespace Orckestra.Composer.Search.Providers
{
	public abstract class BaseSearchUrlProvider
	{
		/// <summary>
		/// Converts the search criteria to a querystring.
		/// </summary>
		/// <returns></returns>
		public NameValueCollection BuildSearchQueryString(BuildSearchUrlParam param)
		{
			var queryString = new NameValueCollection();
			if (param.SearchCriteria == null)
			{
				return queryString;
			}

		    if (!String.IsNullOrEmpty(param.CorrectedSearchTerms))
		    {
		        queryString.Add("keywords", param.CorrectedSearchTerms);
		    }
            else if (!string.IsNullOrEmpty(param.SearchCriteria.Keywords))
			{
                queryString.Add("keywords", param.SearchCriteria.Keywords);
			}

            if (!string.IsNullOrEmpty(param.SearchCriteria.SortBy))
			{
                queryString.Add("sortBy", param.SearchCriteria.SortBy);
			}

            if (!string.IsNullOrEmpty(param.SearchCriteria.SortDirection))
			{
                queryString.Add("sortDirection", param.SearchCriteria.SortDirection);
			}

            if (param.SearchCriteria.Page >= 1)
			{
                queryString.Add("page", param.SearchCriteria.Page.ToString(CultureInfo.InvariantCulture));
			}

            if (param.SearchCriteria.SelectedFacets != null && param.SearchCriteria.SelectedFacets.Count > 0)
			{
                for (var i = 0; i < param.SearchCriteria.SelectedFacets.Count; i++)
				{
                    var currentFilter = param.SearchCriteria.SelectedFacets[i];
					var indexForQueryStringKey = (i + 1).ToString(CultureInfo.InvariantCulture);
					queryString.Add(SearchConfiguration.FilterNameParameterPrefix + indexForQueryStringKey, currentFilter.Name);
					queryString.Add(SearchConfiguration.FilterValueParameterPrefix + indexForQueryStringKey, currentFilter.Value);
				}
			}

			return queryString;
		}

		/// <summary>
		/// Builds the selected facets.
		/// </summary>
		/// <param name="queryString">The query string.</param>
		/// <returns></returns>
		public IEnumerable<SearchFilter> BuildSelectedFacets(NameValueCollection queryString)
		{
			if (queryString == null)
			{
				return null;
			}

			// TODO use NameValue collection directly.
			var queryStringTokens = queryString.ToString().Split('&');

			var filterNameTokens = queryStringTokens
				.Where(x => x.StartsWith(SearchConfiguration.FilterNameParameterPrefix, StringComparison.InvariantCultureIgnoreCase))
				.OrderBy(x => x)
				.Select(x => x.Split('='))
				.ToList();

			var filterValueTokens = queryStringTokens
				.Where(x => x.StartsWith(SearchConfiguration.FilterValueParameterPrefix, StringComparison.InvariantCultureIgnoreCase))
				.OrderBy(x => x)
				.Select(x => x.Split('='))
				.ToList();

			var filters = new List<SearchFilter>();

			foreach (var filterNameToken in filterNameTokens)
			{
				var filterPosition = Regex.Replace(filterNameToken[0], "[A-Za-z=]", string.Empty);
				var filterValueToken = filterValueTokens
					.FirstOrDefault(x => x[0].Equals(SearchConfiguration.FilterValueParameterPrefix + filterPosition, StringComparison.InvariantCultureIgnoreCase));

				if (filterValueToken == null)
				{
					continue;
				}

				filters.Add(new SearchFilter
				{
					Name = HttpUtility.UrlDecode(filterNameToken[1]),
					// TODO: Split here and support list of values here instead of doing it when building facet predicates.
					Value = HttpUtility.UrlDecode(filterValueToken[1])
				});
			}

			return filters;
		}
   
	}
}
