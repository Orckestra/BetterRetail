using Orckestra.Composer.Parameters;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Providers
{
    public interface IBaseSearchCriteriaProvider
    {
        Task<SearchCriteria> GetSearchCriteria(string searchTerms, int limit, int offset, string baseURL, bool includeFacets);
    }
}