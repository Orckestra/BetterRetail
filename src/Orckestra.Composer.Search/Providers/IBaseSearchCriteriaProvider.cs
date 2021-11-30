using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.RequestConstants;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Providers
{
    public interface IBaseSearchCriteriaProvider
    {
        Task<SearchCriteria> GetSearchCriteriaAsync(string searchTerms, int limit, int offset, string baseURL, bool includeFacets, int page = 1, string sortBy = null, string sortDirection = SearchRequestParams.DefaultSortDirection);
    }
}