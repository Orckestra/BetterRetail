using Orckestra.Composer.Repositories;
using Orckestra.Overture.ServiceModel.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Repositories
{
    public interface ISearchCategoryRepository : ICategoryRepository
    {
		Task<List<Facet>> GetCategoryProductCount(string scopeId, string cultureName);

		Task<List<Facet>> GetBrandProductCount(string scopeId, string cultureName);
	}
}