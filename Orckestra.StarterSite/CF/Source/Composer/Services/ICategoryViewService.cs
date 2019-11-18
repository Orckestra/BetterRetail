using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Services
{
    public interface ICategoryViewService
    {
        /// <summary>
        /// Gets the categories path from the provided categoryId to the root category.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        Task<CategoryViewModel[]> GetCategoriesPathAsync(GetCategoriesPathParam param);
    }
}
