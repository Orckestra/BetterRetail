using Orckestra.Composer.Repositories;
using System.Collections.Generic;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using System.Threading.Tasks;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Search.Repositories
{
    public class SearchCategoryRepository : CategoryRepository, ISearchCategoryRepository
    {
        public SearchCategoryRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider) : base(overtureClient, cacheProvider)
        {
        }

		public Task<List<Facet>> GetCategoryProductCount(string scopeId, string cultureName)
		{
			return Task.FromResult(OvertureClient.Send(new AdvancedSearchRequest
			{
				CultureName = cultureName,
				IndexName = "Products",
				ScopeId = scopeId,
				SearchTerms = "*",
				IncludeFacets = true,
				FacetHierarchyId = "CategoryAutoSuggest",
				Query = new Query
				{
					MaximumItems = 0,
					Filter = new FilterGroup
					{
						BinaryOperator = BinaryOperator.And,
						Filters = new List<Filter>
						{
							new Filter
							{
								Member = "CatalogId",
								Operator = Operator.Equals,
								Value = scopeId
							},
							new Filter
							{
								Member = "Active",
								Operator = Operator.Equals,
								Value = true
							}
						}
					}
				}
			}).Facets);
		}

		public Task<List<Facet>> GetBrandProductCount(string scopeId, string cultureName)
		{
			return Task.FromResult(OvertureClient.Send(new AdvancedSearchRequest
			{
				CultureName = cultureName,
				IndexName = "Products",
				ScopeId = scopeId,
				SearchTerms = "*",
				IncludeFacets = true,
				FacetHierarchyId = "BrandAutoSuggest",
				Query = new Query
				{
					MaximumItems = 0,
					Filter = new FilterGroup
					{
						BinaryOperator = BinaryOperator.And,
						Filters = new List<Filter>
						{
							new Filter
							{
								Member = "CatalogId",
								Operator = Operator.Equals,
								Value = scopeId
							},
							new Filter
							{
								Member = "Active",
								Operator = Operator.Equals,
								Value = true
							}
						}
					}
				}
			}).Facets);
		}
	}
}