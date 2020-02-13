using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.SearchQuery.Context;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace Orckestra.Composer.C1CMS.Queries.Controllers
{
    public class SearchQueryController : Controller
    {
        protected IComposerContext ComposerContext { get; }
        protected ISearchQueryContext SearchQueryContext { get; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }

        protected SearchQueryType QueryType = SearchQueryType.Merchandising;

        public SearchQueryController(
            IComposerContext composerContext,
            ISearchUrlProvider searchUrlProvider,
            ISearchQueryContext searchQueryContext,
            IInventoryLocationProvider inventoryLocationProvider
            )
        {
            ComposerContext = composerContext;
            SearchUrlProvider = searchUrlProvider;
            SearchQueryContext = searchQueryContext;
            InventoryLocationProvider = inventoryLocationProvider;
        }

        public virtual ActionResult Top(string queryName = "", int number = 0)
        {
            var searchQueryViewModel =
                SearchQueryContext.GetSearchQueryViewModelAsync(BuildParameters(queryName, 1, null, null, number)).Result;

            if (searchQueryViewModel.ProductSearchResults.Pagination != null)
            {
                searchQueryViewModel.ProductSearchResults.Pagination.TotalNumberOfPages = 1;
                searchQueryViewModel.ProductSearchResults.Pagination.NextPage = null;
            }

            return View("SearchQueryTopResults", searchQueryViewModel);
        }

        public virtual ActionResult Index(string queryName = "", int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteSearchQuery("SearchResults", "SearchResults", c => c, queryName, page, sortBy, sortDirection);
        }

        public virtual ActionResult SelectedSearchFacets(string queryName = "", int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteSearchQuery("SelectedSearchFacets", "SelectedSearchFacets", c => c, queryName, page, sortBy, sortDirection);
        }

        public virtual ActionResult Facets(string queryName = "", int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteSearchQuery("SearchFacetsEmpty", "SearchFacets", c => c.ProductSearchResults, queryName, page, sortBy, sortDirection);
        }

        public virtual ActionResult ChildCategories(string queryName = "", int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteSearchQuery("ChildCategories", "ChildCategories", c => c, queryName, page, sortBy, sortDirection);
        }

        protected ActionResult ExecuteSearchQuery(string emptyView, string filledView,
            Func<SearchQueryViewModel, object> viewModelSelector, string queryName, int page,
            string sortBy = null, string sortDirection = null)
        {
            var param = BuildParameters(queryName, page, sortBy, sortDirection, SearchConfiguration.MaxItemsPerPage);
            var container =
               SearchQueryContext.GetSearchQueryViewModelAsync(param).Result;

            var viewName = container.ProductSearchResults.TotalCount <= 0 ? emptyView : filledView;

            var model = viewModelSelector.Invoke(container);
            return View(viewName, model);
        }

        protected virtual GetSearchQueryViewModelParams BuildParameters(string queryName, int page, string sortBy, string sortDirection, int maxItemsPerPage)
        {
            var searchCriteria = new SearchCriteria
            {
                NumberOfItemsPerPage = maxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (page - 1) * maxItemsPerPage,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                InventoryLocationIds = GetInventoryLocationIds()
            };

            searchCriteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(Request.QueryString));

            var param = new GetSearchQueryViewModelParams
            {
                Criteria = searchCriteria,
                QueryName = queryName,
                QueryType = QueryType,
                InventoryLocationIds = GetInventoryLocationIds(),
            };

            return param;
        }

        protected virtual List<string> GetInventoryLocationIds()
        {
            var ids = InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result;
            return ids;
        }
    }
}
