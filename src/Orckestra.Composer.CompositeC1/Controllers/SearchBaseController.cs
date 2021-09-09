using System;
using System.Globalization;
using System.Web.Mvc;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.CompositeC1.Controllers.Helpers;
using Composite.Data;
using Orckestra.Composer.Search.RequestConstants;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class SearchBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IPageService PageService { get; private set; }
        protected ISearchRequestContext SearchRequestContext { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected ISearchBreadcrumbViewService SearchBreadcrumbViewService { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }

        protected SearchBaseController(
            IComposerContext composerContext,
            IPageService pageService,
            ISearchRequestContext searchRequestContext,
            ILanguageSwitchService languageSwitchService,
            ISearchBreadcrumbViewService searchBreadcrumbViewService,
            ISearchUrlProvider searchUrlProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            SearchRequestContext = searchRequestContext ?? throw new ArgumentNullException(nameof(searchRequestContext));
            LanguageSwitchService = languageSwitchService ?? throw new ArgumentNullException(nameof(languageSwitchService));
            SearchBreadcrumbViewService = searchBreadcrumbViewService ?? throw new ArgumentNullException(nameof(searchBreadcrumbViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
        }

        public virtual ActionResult Index(
            [Bind(Prefix = SearchRequestParams.Keywords)]string keywords, 
            [Bind(Prefix = SearchRequestParams.Page)]int page = 1, 
            [Bind(Prefix = SearchRequestParams.SortBy)]string sortBy = null, 
            [Bind(Prefix = SearchRequestParams.SortDirection)]string sortDirection = SearchRequestParams.DefaultSortDirection)
        {
            if (!AreKeywordsValid(keywords))
            {
                return View("ProductsSearchResults");
            }

            var searchViewModel = SearchRequestContext.ProductsSearchViewModel;

            searchViewModel.Context["SearchResults"] = searchViewModel.ProductSearchResults.SearchResults;
            searchViewModel.Context["Keywords"] = searchViewModel.ProductSearchResults.Keywords;
            searchViewModel.Context["TotalCount"] = searchViewModel.ProductSearchResults.TotalCount;
            searchViewModel.Context["MaxItemsPerPage"] = SearchConfiguration.MaxItemsPerPage;
            searchViewModel.Context["ListName"] = "Search Results";
            searchViewModel.Context["PaginationCurrentPage"] = searchViewModel.ProductSearchResults.Pagination.CurrentPage;

            return View("ProductsSearchResults", searchViewModel);
        }

        public virtual ActionResult SelectedSearchFacets(
            [Bind(Prefix = SearchRequestParams.Keywords)]string keywords, 
            [Bind(Prefix = SearchRequestParams.Page)]int page = 1, 
            [Bind(Prefix = SearchRequestParams.SortBy)]string sortBy = null, 
            [Bind(Prefix = SearchRequestParams.SortDirection)]string sortDirection = SearchRequestParams.DefaultSortDirection)
        {
            var searchViewModel = GetSearchViewModel();
            
            return View("SelectedSearchFacets", searchViewModel);
        }

        public virtual ActionResult SearchFacets(
			[Bind(Prefix = SearchRequestParams.Keywords)]string keywords, 
            [Bind(Prefix = SearchRequestParams.Page)]int page = 1, 
            [Bind(Prefix = SearchRequestParams.SortBy)]string sortBy = null, 
            [Bind(Prefix = SearchRequestParams.SortDirection)]string sortDirection = SearchRequestParams.DefaultSortDirection)
        {
            if (!AreKeywordsValid(keywords))
            {
                return View("SearchFacetsEmpty");
            }

            var searchViewModel = GetSearchViewModel();

            return searchViewModel.ProductSearchResults.TotalCount == 0
                ? View("SearchFacetsEmpty")
                : View("SearchFacets", searchViewModel.ProductSearchResults);
        }
 
        protected virtual bool AreKeywordsValid(string keywords)
        {
            return SearchControllerHelper.AreKeywordsValid(keywords);
        }

        protected virtual SearchViewModel GetSearchViewModel()
        {
                return SearchRequestContext.ProductsSearchViewModel;
        }

        public virtual ActionResult PageHeader(string keywords)
        {
            var pageHeaderViewModel = SearchRequestContext.GetPageHeaderViewModelAsync(new GetPageHeaderParam
            {
                Keywords = keywords,
                IsPageIndexed = IsPageIndexed()

            }).Result;

            return View(pageHeaderViewModel);
        }

        protected virtual bool IsPageIndexed()
        {
            return !Request.QueryString.HasKeys();
        }
    }
}
