using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Repositories
{
	//TODO: Fix me please
    public interface ICategoryRepository
    {
        /// <summary>
        /// Gets the categories path from the provided categoryId to the root category.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// Categories path starting at provided category up to root category.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        Task<List<Category>> GetCategoriesPathAsync(GetCategoriesPathParam param);


        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>List of categories.</returns>
        Task<List<Category>> GetCategoriesAsync(GetCategoriesParam param);

        /// <summary>
        /// Gets a tree of categories.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<Tree<Category, string>> GetCategoriesTreeAsync(GetCategoriesParam param);
    }
}