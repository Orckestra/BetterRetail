using Orckestra.Composer.SearchQuery.ViewModels;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System.Threading.Tasks;

namespace Orckestra.Composer.SearchQuery.Context
{
    public interface ISearchQueryContext
    {
        Task<SearchQueryViewModel> GetSearchQueryViewModelAsync(string queryTypeValue, string queryName);
        Task<SearchQueryViewModel> GetSearchQueryViewModelAsync(SearchQueryType queryType, string queryName);
        Task<SearchQueryViewModel> GetTopSearchQueryViewModelAsync(string queryTypeValue, string queryName, int pageSize);
        Task<SearchQueryViewModel> GetTopSearchQueryViewModelAsync(SearchQueryType queryType, string queryName, int pageSize);
   }
}
