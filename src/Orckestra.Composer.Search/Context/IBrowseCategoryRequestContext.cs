using Orckestra.Composer.Search.ViewModels;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Context
{
    public interface IBrowseCategoryRequestContext
    {
        /// <summary>
        /// Get CategoryBrowsingViewModel for the current request
        /// </summary>
        Task<CategoryBrowsingViewModel> GetViewModelAsync();
    }
}
