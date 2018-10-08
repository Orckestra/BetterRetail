using System.Threading.Tasks;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.ViewModels;

namespace Orckestra.Composer.Search.Services
{
    public interface ICategoryBrowsingViewService
    {
        Task<CategoryBrowsingViewModel> GetCategoryBrowsingViewModelAsync(GetCategoryBrowsingViewModelParam param);
    }
}