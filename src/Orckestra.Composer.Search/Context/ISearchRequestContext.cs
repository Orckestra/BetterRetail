using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public interface ISearchRequestContext
    {
        SearchViewModel ProductsSearchViewModel { get; }
        bool IsProductsSearchActive { get; set; }
        int CurrentPage { get;  }
        string SearchQuery { get; }

        string SortBy { get; }

        string SortDirection { get; }
        Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param);
    }
}