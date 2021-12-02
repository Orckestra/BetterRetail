using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.RequestConstants;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Providers
{
    public interface IBaseSearchCriteriaProvider
    {
        Task<SearchCriteria> GetSearchCriteriaAsync(string searchTerms, string baseURL, bool includeFacets, int page = 1);
    }
}