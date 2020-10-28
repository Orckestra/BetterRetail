using Orckestra.Composer.SearchQuery.Context;
using Orckestra.Composer.SearchQuery.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System;
using System.Web.Mvc;


namespace Orckestra.Composer.C1CMS.Queries.Controllers
{
    public class SearchQueryController : Controller
    {
        protected IComposerContext ComposerContext { get; }
        protected ISearchQueryContext SearchQueryContext { get; }

        protected SearchQueryType QueryType = SearchQueryType.Merchandising;

        public SearchQueryController(
            IComposerContext composerContext,
            ISearchQueryContext searchQueryContext
            )
        {
            ComposerContext = composerContext;
            SearchQueryContext = searchQueryContext;
        }

        public virtual ActionResult Index(string queryName = "")
        {
            return ExecuteSearchQuery("SearchResults", "SearchResults", c => c, queryName);
        }

        public virtual ActionResult SelectedSearchFacets(string queryName = "")
        {
            return ExecuteSearchQuery("SelectedSearchFacets", "SelectedSearchFacets", c => c, queryName);
        }

        public virtual ActionResult Facets(string queryName = "", int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteSearchQuery("SearchFacetsEmpty", "SearchFacets", c => c.ProductSearchResults, queryName);
        }

        public virtual ActionResult ChildCategories(string queryName = "", int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteSearchQuery("ChildCategories", "ChildCategories", c => c, queryName);
        }

        protected ActionResult ExecuteSearchQuery(string emptyView, string filledView,
            Func<SearchQueryViewModel, object> viewModelSelector, string queryName)
        {
            var container =
               SearchQueryContext.GetSearchQueryViewModelAsync(QueryType, queryName).Result;

            var viewName = container.ProductSearchResults.TotalCount <= 0 ? emptyView : filledView;

            var model = viewModelSelector.Invoke(container);
            return View(viewName, model);
        }
    }
}
