using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;
using ServiceStack;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        protected IOvertureClient OvertureClient { get; }
        protected IProductRequestFactory ProductRequestFactory { get; }
        protected IFacetPredicateFactory FacetPredicateFactory { get; }
        protected IFacetConfigurationContext FacetConfigContext { get; }

        public SearchRepository(
            IOvertureClient overtureClient,
            IProductRequestFactory productRequestFactory,
            IFacetPredicateFactory facetPredicateFactory,
            IFacetConfigurationContext facetConfigContext)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            ProductRequestFactory = productRequestFactory ?? throw new ArgumentNullException(nameof(productRequestFactory));
            FacetPredicateFactory = facetPredicateFactory ?? throw new ArgumentNullException(nameof(facetPredicateFactory));
            FacetConfigContext = facetConfigContext ?? throw new ArgumentNullException(nameof(facetConfigContext));
        }

        public virtual async Task<ProductSearchResult> SearchProductAsync(SearchCriteria criteria)
        {
            if (criteria == null) { throw new ArgumentNullException(nameof(criteria)); }
            if (criteria.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(criteria.CultureInfo)), nameof(criteria)); }
            if (string.IsNullOrWhiteSpace(criteria.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(criteria.Scope)), nameof(criteria)); }

            var request = CreateSearchRequest(criteria);

            var results = await ExecuteProductSearchRequestAsync(request).ConfigureAwait(false);

            if (results?.Facets == null) { return results; }

            var facetsToQuery = GetFacetFieldNameToQuery(criteria);
            results.Facets.RemoveAll(facet => !facetsToQuery.Contains(facet.FieldName));

            if (criteria.SelectedFacets.Count == 0) { return results; }

            var param = new RemoveSelectedFacetsFromFacetsParam
            {
                Facets = results.Facets,
                SelectedFacets = criteria.SelectedFacets,
                FacetSettings = FacetConfigContext.GetFacetSettings()
            };

            results.Facets = RemoveSelectedFacetsFromFacets(param);

            return results;
        }


        protected virtual async Task<ProductSearchResult> ExecuteProductSearchRequestAsync(SearchAvailableProductsBaseRequest request)
        {
            if (request is IReturn<ProductSearchResult> returnProductSerachResult)
            {
                return await OvertureClient.SendAsync(returnProductSerachResult).ConfigureAwait(false);
            }

            if (request is IReturn<SearchAvailableProductsByCategoryResponse> returnSearchAvailableProductsByCategoryResponse)
            {
                return await OvertureClient.SendAsync(returnSearchAvailableProductsByCategoryResponse).ConfigureAwait(false);
            }
            return null;
        }

        protected virtual List<Facet> RemoveSelectedFacetsFromFacets(RemoveSelectedFacetsFromFacetsParam param)
        {
            var strippedFacets = new List<Facet>();
            var facets = param.Facets;
            var selectedFacets = param.SelectedFacets;
            var facetSettings = param.FacetSettings;

            foreach (var facet in facets)
            {
                var facetSetting = facetSettings.FirstOrDefault(setting => setting.FieldName == facet.FieldName);

                if (facetSetting?.FacetType == Facets.FacetType.MultiSelect || selectedFacets.Find(selectedFacet => selectedFacet.Name == facet.FieldName) == null)
                {
                    strippedFacets.Add(facet);
                }
            }

            return strippedFacets;
        }

        protected virtual SearchAvailableProductsBaseRequest CreateSearchRequest(SearchCriteria criteria)
        {
            var request = ProductRequestFactory.CreateProductRequest(criteria);

            request.Query.IncludeTotalCount = true;
            request.Query.MaximumItems = criteria.NumberOfItemsPerPage;
            request.Query.StartingIndex = (criteria.Page - 1) * criteria.NumberOfItemsPerPage;
            request.CultureName = criteria.CultureInfo.Name;
            request.SearchTerms = criteria.Keywords;
            request.ScopeId = criteria.Scope;
            request.IncludeFacets = criteria.IncludeFacets;
            request.Facets = GetFacetFieldNameToQuery(criteria);
            request.FacetPredicates = BuildFacetPredicates(criteria);
            request.InventoryLocationIds = criteria.InventoryLocationIds;
            request.AutoCorrect = criteria.AutoCorrect;

            var sortDefinitions = BuildQuerySortings(criteria);

            if (sortDefinitions != null)
            {
                request.Query.Sortings.Add(sortDefinitions);
            }

            return request;
        }

        /// <summary>
        /// Build the list of facets to query.
        /// To configure those facets <see cref="SearchConfiguration.FacetSettings"/>
        /// Regardless of what facets are returned by Overture, there are further filtered to match this list.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual List<string> GetFacetFieldNameToQuery(SearchCriteria criteria)
        {
            var facets = new List<string>();

            var requestedFieldNames = criteria.SelectedFacets.Select(facet => facet.Name).ToList();

            if (criteria.IncludeFacets)
            {
                facets
                    .AddRange(FacetConfigContext.GetFacetSettings()
                    .Where(settings => !settings.DependsOn.Any() ||
                                        settings.DependsOn.Intersect(requestedFieldNames).Any())
                    .Select(settings => settings.FieldName));
            }

            return facets;
        }

        protected virtual List<FacetPredicate> BuildFacetPredicates(SearchCriteria criteria)
        {
            var facetPredicates = new List<FacetPredicate>();

            if (criteria.SelectedFacets != null)
            {
                facetPredicates.AddRange(criteria.SelectedFacets
                    .Select(FacetPredicateFactory.CreateFacetPredicate)
                    .Where(fp => fp != null).ToList());
            }

            return facetPredicates;
        }

        protected virtual QuerySorting BuildQuerySortings(SearchCriteria criteria)
        {
            if (string.IsNullOrWhiteSpace(criteria.SortBy)) { return null; }

            var sortDirection = 
                string.IsNullOrWhiteSpace(criteria.SortDirection) || criteria.SortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase)
                ? SortDirection.Ascending
                : SortDirection.Descending;

            return new QuerySorting
            {
                Direction = sortDirection,
                PropertyName = criteria.SortBy
            };
        }
    }
}