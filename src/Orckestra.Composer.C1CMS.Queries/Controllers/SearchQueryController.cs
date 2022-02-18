using Orckestra.Composer.SearchQuery.Context;
using Orckestra.Composer.SearchQuery.ViewModels;
using Orckestra.Overture.ServiceModel.SearchQueries;
using System;
using System.Web.Mvc;


namespace Orckestra.Composer.C1CMS.Queries.Controllers
{
    public class SearchQueryController : Controller
    {
        protected ISearchQueryContext SearchQueryContext { get; }

        protected SearchQueryType QueryType = SearchQueryType.Merchandising;

        public SearchQueryController(ISearchQueryContext searchQueryContext)
        {
            SearchQueryContext = searchQueryContext;
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
