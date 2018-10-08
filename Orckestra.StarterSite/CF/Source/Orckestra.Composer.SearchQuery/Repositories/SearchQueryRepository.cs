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

namespace Orckestra.Composer.SearchQuery.Repositories
{
    public class SearchQueryRepository: SearchRepository, ISearchQueryRepository
    {

        public SearchQueryRepository(IOvertureClient overtureClient,
            IProductRequestFactory productRequestFactory,
            IFacetPredicateFactory facetPredicateFactory) : base(overtureClient, productRequestFactory, facetPredicateFactory)
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

        public async Task<SearchQueryResult> SearchQueryProductAsync(SearchQueryProductParams param)
        {
            var criteria = param.Criteria;

            var request = new SearchBySearchQueryRequest()
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

    }
}
