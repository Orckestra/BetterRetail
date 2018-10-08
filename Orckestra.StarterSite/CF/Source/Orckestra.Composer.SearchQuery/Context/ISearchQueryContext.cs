using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.ViewModels;
using System.Threading.Tasks;

namespace Orckestra.Composer.SearchQuery.Context
{
    public interface ISearchQueryContext
    {
        Task<SearchQueryViewModel> GetSearchQueryViewModelAsync(GetSearchQueryViewModelParams criteria);
    }
}
