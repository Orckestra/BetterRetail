using System;
using System.Collections.Generic;
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
using System.Linq;
using Orckestra.ExperienceManagement.Configuration;
using Composite.Data;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public abstract class SearchBaseController : Controller
    {
        private const string DefaultSortDirection = "asc";

        protected IComposerContext ComposerContext { get; private set; }
        protected IPageService PageService { get; private set; }
        protected ISearchRequestContext SearchRequestContext { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected ISearchUrlProvider UrlProvider { get; private set; }
        protected ISearchBreadcrumbViewService SearchBreadcrumbViewService { get; private set; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }
        protected ISiteConfiguration SiteConfiguration { get; private set; }
        protected PagesConfiguration PagesConfiguration { get; private set; }

        protected SearchBaseController(
            IComposerContext composerContext,
            IPageService pageService,
            ISearchRequestContext searchRequestContext,
            ILanguageSwitchService languageSwitchService,
            ISearchUrlProvider urlProvider,
            ISearchBreadcrumbViewService searchBreadcrumbViewService,
            IInventoryLocationProvider inventoryLocationProvider,
            ISearchUrlProvider searchUrlProvider,
            ISiteConfiguration siteConfiguration)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (pageService == null) { throw new ArgumentNullException("pageService"); }
            if (searchRequestContext == null) { throw new ArgumentNullException("searchRequestContext"); }
            if (languageSwitchService == null) { throw new ArgumentNullException("languageSwitchService"); }
            if (urlProvider == null) { throw new ArgumentNullException("urlProvider"); }
            if (searchBreadcrumbViewService == null) { throw new ArgumentNullException("searchBreadcrumbViewService"); }
            if (inventoryLocationProvider == null) { throw new ArgumentNullException("inventoryLocationProvider"); }
            if (searchUrlProvider == null) { throw new ArgumentNullException("searchUrlProvider"); }

            ComposerContext = composerContext;
            PageService = pageService;
            SearchRequestContext = searchRequestContext;
            LanguageSwitchService = languageSwitchService;
            UrlProvider = urlProvider;
            SearchBreadcrumbViewService = searchBreadcrumbViewService;
            InventoryLocationProvider = inventoryLocationProvider;
            SearchUrlProvider = searchUrlProvider;
            SiteConfiguration = siteConfiguration;
            PagesConfiguration = siteConfiguration.GetPagesConfiguration();
        }

        public virtual ActionResult SearchBox(string keywords)
        {
            var searchBoxViewModel = new SearchBoxViewModel
            {
                Keywords = keywords ?? string.Empty,
                ActionTarget = PageService.GetRendererPageUrl(PagesConfiguration.SearchPageId, ComposerContext.CultureInfo)
            };

            return View(searchBoxViewModel);
        }

        public virtual ActionResult Index(string keywords, int page = 1, string sortBy = null, string sortDirection = DefaultSortDirection)
        {
            if (!AreKeywordsValid(keywords))
            {
                return View("SearchResults");
            }

            var searchViewModel = GetSearchViewModel(keywords, page, sortBy, sortDirection);

            searchViewModel.Context["SearchResults"] = searchViewModel.ProductSearchResults.SearchResults;
            searchViewModel.Context["Keywords"] = searchViewModel.ProductSearchResults.Keywords;
            searchViewModel.Context["TotalCount"] = searchViewModel.ProductSearchResults.TotalCount;
            searchViewModel.Context["MaxItemsPerPage"] = SearchConfiguration.MaxItemsPerPage;
            searchViewModel.Context["ListName"] = "Search Results";
            searchViewModel.Context["PaginationCurrentPage"] = searchViewModel.ProductSearchResults.Pagination.Pages.FirstOrDefault(p => p.IsCurrentPage);


            return View("SearchResults", searchViewModel);
        }

        public virtual ActionResult SelectedSearchFacets(string keywords, int page = 1, string sortBy = null, string sortDirection = DefaultSortDirection)
        {
            var searchViewModel = GetSearchViewModel(keywords, page, sortBy, sortDirection);
            
            return View("SelectedSearchFacets", searchViewModel);
        }

        public virtual ActionResult SearchFacets(string keywords, int page = 1, string sortBy = null, string sortDirection = DefaultSortDirection)
        {
            if (!AreKeywordsValid(keywords))
            {
                return View("SearchFacetsEmpty");
            }

            var searchViewModel = GetSearchViewModel(keywords, page, sortBy, sortDirection);

            return searchViewModel.ProductSearchResults.TotalCount == 0
                ? View("SearchFacetsEmpty")
                : View("SearchFacets", searchViewModel.ProductSearchResults);
        }

        public virtual ActionResult SearchSummary(string keywords, int page = 1, string sortBy = null, string sortDirection = DefaultSortDirection)
        {
            if (!AreKeywordsValid(keywords))
            {
                return View("SearchSummary", new SearchViewModel { Keywords = keywords });
            }

            var searchViewModel = GetSearchViewModel(keywords, page, sortBy, sortDirection);

            searchViewModel.Context["TotalCount"] = searchViewModel.ProductSearchResults.TotalCount;
            searchViewModel.Context["Keywords"] = searchViewModel.ProductSearchResults.Keywords;
            searchViewModel.Context["CorrectedSearchTerms"] = searchViewModel.ProductSearchResults.CorrectedSearchTerms;
            searchViewModel.Context["ListName"] = "Search Results";

            return View("SearchSummary", searchViewModel);
        }

        protected virtual bool AreKeywordsValid(string keywords)
        {
            if (String.IsNullOrWhiteSpace(keywords))
            {
                return false;
            }

            var strippedKeywords = keywords.Trim();

            if (SearchConfiguration.BlockStarSearchs)
            {
                strippedKeywords = strippedKeywords.Replace("*", "");
            }

            var isInvalid = String.IsNullOrWhiteSpace(strippedKeywords);
            return !isInvalid;
        }

        protected virtual SearchViewModel GetSearchViewModel(string keywords, int page, string sortBy, string sortDirection)
        {
            var criteria = new SearchCriteria
            {
                Keywords = keywords,
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (page - 1) * SearchConfiguration.MaxItemsPerPage, // the starting index is zero-based
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                InventoryLocationIds = GetInventoryLocationIds(),
                AutoCorrect = SearchConfiguration.AutoCorrectSearchTerms
            };

            criteria.SelectedFacets.AddRange(UrlProvider.BuildSelectedFacets(Request.QueryString));

            return SearchRequestContext.GetSearchViewModelAsync(criteria).Result;
        }

        protected virtual List<string> GetInventoryLocationIds()
        {
            var ids = InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result;

            return ids;
        } 

        public virtual ActionResult Breadcrumb(string keywords)
        {
            var breadcrumbViewModel = SearchBreadcrumbViewService.CreateBreadcrumbViewModel(new GetSearchBreadcrumbParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                HomeUrl = PageService.GetRendererPageUrl(SitemapNavigator.CurrentHomePageId, ComposerContext.CultureInfo),
                Keywords = keywords
            });

            return View("Breadcrumb", breadcrumbViewModel);
        }

        public virtual ActionResult LanguageSwitch(string keywords, int page = 1, string sortBy = null, string sortDirection = null)
        {
            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(cultureInfo =>
                BuildUrl(cultureInfo, keywords, page, sortBy, sortDirection), ComposerContext.CultureInfo);

            return View("LanguageSwitch", languageSwitchViewModel);
        }

        private string BuildUrl(CultureInfo cultureInfo, string keywords, int page, string sortBy, string sortDirection)
        {
            var searchUrl = UrlProvider.BuildSearchUrl(new BuildSearchUrlParam
            {
                SearchCriteria = new SearchCriteria
                {
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = cultureInfo,
                Keywords = keywords,
                //We have to return to page 1 because the number of results can be different from a language to another
                Page = 1,
                SortBy = sortBy,
                SortDirection = sortDirection
                }
            });

            return searchUrl;
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
