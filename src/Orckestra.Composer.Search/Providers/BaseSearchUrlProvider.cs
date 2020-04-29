using System;
using System.Collections.Specialized;
using System.Globalization;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.RequestConstants;

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
            if (param.SearchCriteria == null) { return queryString; }

            if (!string.IsNullOrEmpty(param.CorrectedSearchTerms))
            {
                queryString.Add(SearchRequestParams.Keywords, param.CorrectedSearchTerms);
            }
            else if (!string.IsNullOrEmpty(param.SearchCriteria.Keywords))
            {
                queryString.Add(SearchRequestParams.Keywords, param.SearchCriteria.Keywords);
            }

            if (!string.IsNullOrEmpty(param.SearchCriteria.SortBy))
            {
                queryString.Add(SearchRequestParams.SortBy, param.SearchCriteria.SortBy);
            }

            if (!string.IsNullOrEmpty(param.SearchCriteria.SortDirection))
            {
                queryString.Add(SearchRequestParams.SortDirection, param.SearchCriteria.SortDirection);
            }

            if (param.SearchCriteria.Page > 0)
            {
                queryString.Add(SearchRequestParams.Page, param.SearchCriteria.Page.ToString(CultureInfo.InvariantCulture));
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
    };
}