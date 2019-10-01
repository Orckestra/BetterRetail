using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public interface ISearchRequestContext
    {
        Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param);

        Task<SearchViewModel> GetSearchViewModelAsync(SearchCriteria criteria);
        Task<SearchViewModel> GetSearchViewModelAsync(GetSearchViewModelParam param);
    }
}