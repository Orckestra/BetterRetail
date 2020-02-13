using System.Threading.Tasks;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public interface IBrowseCategoryRequestContext
    {
        /// <summary>
        /// Gets all products for a given category.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CategoryBrowsingViewModel> GetCategoryAvailableProductsAsync(GetBrowseCategoryParam param);
    }
}
