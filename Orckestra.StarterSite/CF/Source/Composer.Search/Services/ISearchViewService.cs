using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;
using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Search.Services
{
    public interface ISearchViewService
    {
        Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param);

        Task<SearchViewModel> GetSearchViewModelAsync(SearchCriteria criteria);

        Task<List<Category>> GetAllCategories();

        Task<List<Orckestra.Overture.ServiceModel.Search.Facet>> GetCategoryProductCounts(string cultureName);

        Task<List<Orckestra.Overture.ServiceModel.Search.Facet>> GetBrandProductCounts(string cultureName);
    }
}
