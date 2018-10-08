using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.Services
{
    public interface ISearchViewService
    {
        Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param);

        Task<SearchViewModel> GetSearchViewModelAsync(SearchCriteria criteria);
    }
}
