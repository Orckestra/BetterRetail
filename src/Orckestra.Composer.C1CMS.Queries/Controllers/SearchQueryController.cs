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
                SearchQueryContext.GetTopSearchQueryViewModelAsync(QueryType, queryName, number).Result;

            return View("SearchQueryTopResults", searchQueryViewModel);
        }

        public virtual ActionResult Index(string queryName = "")
        {
            return ExecuteSearchQuery("ProductsSearchResults", "ProductsSearchResults", c => c, queryName);
        }

        public virtual ActionResult SelectedSearchFacets(string queryName = "")
        {
            return ExecuteSearchQuery("SelectedSearchFacets", "SelectedSearchFacets", c => c, queryName);
        }

        public virtual ActionResult Facets(string queryName = "")
        {
            return ExecuteSearchQuery("SearchFacetsEmpty", "SearchFacets", c => c.ProductSearchResults, queryName);
        }

        public virtual ActionResult ChildCategories(string queryName = "")
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
