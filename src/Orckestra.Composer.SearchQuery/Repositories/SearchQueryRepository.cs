using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Requests.SearchQueries;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Search.Context;
using Orckestra.Overture.ServiceModel.Search;
using System.Linq;
using Orckestra.Composer.Search;
using System;
using Orckestra.Composer.Parameters;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.SearchQuery.Repositories
{
    public class SearchQueryRepository : SearchRepository, ISearchQueryRepository
    {

        public SearchQueryRepository(IOvertureClient overtureClient,
            IProductRequestFactory productRequestFactory,
            IFacetPredicateFactory facetPredicateFactory,
            IFacetConfigurationContext facetConfigContext)
            : base(overtureClient, productRequestFactory, facetPredicateFactory, facetConfigContext)
        {
        }

        public virtual async Task<FindSearchQueriesResult> GetSearchQueriesAsync(GetSearchQueriesParam param)
        {
            var request = new FindSearchQueriesRequest
            {
                ScopeId = param.Scope,
                QueryType = param.QueryType
            };
            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);
            return result;
        }

        public virtual async Task<Overture.ServiceModel.SearchQueries.SearchQuery> GetSearchQueryAsync(
           SearchQueryAsyncParam param)
        {

            var request = new GetSearchQueryByNameRequest
            {
                ScopeId = param.Scope,
                Name = param.Name,
                QueryType = param.QueryType
            };

            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);

            return result;
        }

        public virtual async Task<SearchQueryResult> SearchQueryProductAsync(SearchQueryProductParams param)
        {
            var criteria = param.Criteria;
            var request = GerSearchQueryRequest(param);

            request.Query.IncludeTotalCount = true;
            request.Query.MaximumItems = criteria.NumberOfItemsPerPage;
            request.Query.StartingIndex = (criteria.Page - 1) * criteria.NumberOfItemsPerPage;
            request.CultureName = criteria.CultureInfo.Name;
            request.SearchTerms = criteria.Keywords;
            request.ScopeId = criteria.Scope;
            request.IncludeFacets = criteria.IncludeFacets;
            request.Facets = GetFacetFieldNameToQuery(criteria);
            request.FacetPredicates = BuildFacetPredicates(criteria);
            request.AutoCorrect = criteria.AutoCorrect;
            var sortDefinitions = BuildQuerySortings(criteria);

            if (sortDefinitions != null)
            {
                request.Query.Sortings.Add(sortDefinitions);
            }

            var result = await OvertureClient.SendAsync(request).ConfigureAwait(false);
            return result;
        }

        private  SearchBySearchQueryRequest GerSearchQueryRequest(SearchQueryProductParams param)
        {
            return new SearchBySearchQueryRequest()
            {
                QueryType = param.QueryType,
                QueryName = param.QueryName,
                ScopeId = param.ScopeId,
                CultureName = param.CultureName,
                Query = new Query
                {
                    Filter = new FilterGroup
                    {
                        BinaryOperator = BinaryOperator.And,
                        Filters = new List<Filter>
                        {
                            new Filter
                            {
                                Member = "CatalogId",
                                Operator = Operator.Equals,
                                Value = param.ScopeId
                            },
                            new Filter
                            {
                                Member = "Active",
                                Operator = Operator.Equals,
                                Value= bool.TrueString
                            }
                        }
                    }
                }
            };
        }

        public Task<ProductSearchResult> GetCategoryFacetCountsAsync(SearchCriteria criteria, SearchQueryResult queryResults)
        {
            if (criteria == null) { throw new ArgumentNullException(nameof(criteria)); }

            var request = ProductRequestFactory.CreateProductRequest(criteria);
            request.Query.IncludeTotalCount = true;
            request.Query.MaximumItems = 0;
            request.Query.StartingIndex = 0;
            request.CultureName = criteria.CultureInfo.Name;
            request.SearchTerms = criteria.Keywords;
            request.ScopeId = criteria.Scope;
            request.IncludeFacets = criteria.IncludeFacets;

            var facetsForCounts = FacetConfigContext.GetFacetSettings()
                .Where(fs => fs.FieldName.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix))
                .Select(f => f.FieldName.Replace("_Facet", ""));
            var facets = GetFacetFieldNameToQuery(criteria);
            facets.AddRange(facetsForCounts);
            request.Facets = facets;
            request.FacetPredicates = new List<FacetPredicate>();
            if (criteria.SelectedFacets != null)
            {
                request.FacetPredicates = criteria.SelectedFacets
                        .Where(sf => !sf.Name.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix))
                        .Select(FacetPredicateFactory.CreateFacetPredicate)
                        .Where(fp => fp != null).ToList();
            }
            if (queryResults.SelectedFacets != null)
            {
                foreach (var queryFacet in queryResults.SelectedFacets)
                {
                    foreach (var queryFacetValue in queryFacet.Values)
                    {
                        if (criteria.SelectedFacets.FirstOrDefault(s => s.Value == queryFacetValue && s.Name == queryFacet.FacetName) == null)
                        {
                            var facetPredicate = FacetPredicateFactory.CreateFacetPredicate(queryFacet.FacetName, queryFacetValue);
                            if (facetPredicate != null)
                            {
                                request.FacetPredicates.Add(facetPredicate);
                            }
                        }
                    }
                }
            }
            request.InventoryLocationIds = criteria.InventoryLocationIds;
            request.AutoCorrect = criteria.AutoCorrect;
            request.AvailabilityDate = criteria.AvailabilityDate;

            return ExecuteProductSearchRequestAsync(request);
        }

    }
}
