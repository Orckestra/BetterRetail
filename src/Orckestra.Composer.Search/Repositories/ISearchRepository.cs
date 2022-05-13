using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Search.Repositories
{
    /// <summary>
    /// Gets or sets the search query factory instance.
    /// </summary>
    /// <value>
    /// The search query factory.
    /// </value>
    public interface  ISearchRepository
    {
        /// <summary>
        /// Searches for products based on the given search critieria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// criteria
        /// or
        /// criteria.CultureInfo
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// criteria.Keywords
        /// or
        /// criteria.Scope
        /// </exception>
        Task<ProductSearchResult> SearchProductAsync(SearchCriteria criteria);

        /// <summary>
        /// Get independent product counts for each configured category facet
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Task<ProductSearchResult> GetCategoryFacetCountsAsync(SearchCriteria criteria);
    }
}
