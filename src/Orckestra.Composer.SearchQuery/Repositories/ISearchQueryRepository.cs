﻿using Orckestra.Composer.Parameters;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Overture.ServiceModel.Search;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System.Threading.Tasks;

namespace Orckestra.Composer.SearchQuery.Repositories
{
    public interface ISearchQueryRepository
    {
        Task<FindSearchQueriesResult> GetSearchQueriesAsync(GetSearchQueriesParam param);

        Task<Overture.ServiceModel.SearchQueries.SearchQuery> GetSearchQueryAsync(SearchQueryAsyncParam param);

        Task<SearchQueryResult> SearchQueryProductAsync(SearchQueryProductParams param);

        Task<ProductSearchResult> GetCategoryFacetCountsAsync(SearchCriteria criteria, SearchQueryResult queryResults);
    }
}
