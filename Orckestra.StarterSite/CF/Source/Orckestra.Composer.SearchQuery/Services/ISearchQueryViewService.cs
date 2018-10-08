using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.ViewModels;
using System.Threading.Tasks;

namespace Orckestra.Composer.SearchQuery.Services
{
    public interface ISearchQueryViewService
    {
        Task<SearchQueryViewModel> GetSearchQueryViewModelAsync(GetSearchQueryViewModelParams param);
    }
}
