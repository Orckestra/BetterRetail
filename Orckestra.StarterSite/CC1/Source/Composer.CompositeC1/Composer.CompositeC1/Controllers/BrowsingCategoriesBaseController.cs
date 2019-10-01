using Composite.Data;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public class BrowsingCategoriesBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }
        protected IBrowseCategoryRequestContext RequestContext { get; private set; }
        protected ICategoryViewService CategoryViewService { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IPageService PageService { get; private set; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; private set; }

        protected static CategoryBrowsingViewModel EmptyCategoryBrowsingContainer { get; set; }

        static BrowsingCategoriesBaseController()
        {
            EmptyCategoryBrowsingContainer = new CategoryBrowsingViewModel
            {
                SelectedFacets = new SelectedFacets { Facets = new List<SelectedFacet>() },
                ProductSearchResults = new ProductSearchResultsViewModel { Keywords = string.Empty },
                ChildCategories = new List<ChildCategoryViewModel>()
            };
        }

        public BrowsingCategoriesBaseController(
            IComposerContext composerContext, 
            ISearchUrlProvider searchUrlProvider, 
            IBrowseCategoryRequestContext requestContext,
            ICategoryViewService categoryViewService, 
            ILanguageSwitchService languageSwitchService, 
            IPageService pageService,
            IInventoryLocationProvider inventoryLocationProvider) 
        {
            ComposerContext = composerContext;
            SearchUrlProvider = searchUrlProvider;
            RequestContext = requestContext;
            CategoryViewService = categoryViewService;
            LanguageSwitchService = languageSwitchService;
            PageService = pageService;
            InventoryLocationProvider = inventoryLocationProvider;
        }

        public virtual ActionResult Summary(int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteBrowsing("CategoryBrowsingSummaryEmpty", "CategoryBrowsingSummary", c => c, null, page, sortBy, sortDirection);
        }

        public virtual ActionResult Index(int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteBrowsing("SearchResults", "SearchResults", c => c, EmptyCategoryBrowsingContainer, page, sortBy, sortDirection);
        }

        public virtual ActionResult Facets(int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteBrowsing("SearchFacetsEmpty", "SearchFacets", c => c.ProductSearchResults, EmptyCategoryBrowsingContainer.ProductSearchResults, page, sortBy, sortDirection);
        }

        public virtual ActionResult ChildCategories(int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteBrowsing("ChildCategories", "ChildCategories", c => c, EmptyCategoryBrowsingContainer, page, sortBy, sortDirection);
        }

        public virtual ActionResult SelectedSearchFacets(int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteBrowsing("SelectedSearchFacets", "SelectedSearchFacets", c => c, EmptyCategoryBrowsingContainer.SelectedFacets, page, sortBy, sortDirection);
        }

        protected ActionResult ExecuteBrowsing(string emptyView, string filledView, Func<CategoryBrowsingViewModel, object> viewModelSelector, object emptyViewModel, int page, string sortBy = null, string sortDirection = null)
        {
            var categoryId = GetCategoryId();
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return View(emptyView, emptyViewModel);
            }

            var param = BuildParameters(categoryId, page, sortBy, sortDirection);
            var container = RequestContext.GetCategoryAvailableProductsAsync(param).Result;

            var viewName = container.ProductSearchResults.TotalCount <= 0 ? emptyView : filledView;
            var model = viewModelSelector.Invoke(container);

            if (model is CategoryBrowsingViewModel)
            {
                ExtendSpecificViewsWithContext(filledView, (CategoryBrowsingViewModel)model);
            }

            return View(viewName, model);
        }

        protected virtual void ExtendSpecificViewsWithContext(string viewName, CategoryBrowsingViewModel model)
        {
            //Add additional viewModel.Context for specific views
            if (viewName == "SearchResults")
            {
                model.Context["SearchResults"] = model.ProductSearchResults.SearchResults;
                model.Context["Keywords"] = model.ProductSearchResults.Keywords;
                model.Context["TotalCount"] = model.ProductSearchResults.TotalCount;
                model.Context["MaxItemsPerPage"] = SearchConfiguration.MaxItemsPerPage;
                model.Context["ListName"] = "Category Browsing";
                model.Context["PaginationCurrentPage"] = model.ProductSearchResults.Pagination.Pages.FirstOrDefault(p => p.IsCurrentPage);
            }
        }

        protected virtual string GetCategoryId()
        {
            var page = PageService.GetPage(SitemapNavigator.CurrentPageId);

            var metaDefName = GetMetadataDefinitionName(page.PageTypeId);


            var categoryPage = page.GetMetaData(metaDefName, typeof(CategoryPage)) as CategoryPage;
            if (categoryPage == null)
            {
                return null;
            }

            return categoryPage == null ? null : categoryPage.CategoryId;
        }

        protected string GetMetadataDefinitionName(Guid pageTypeId)
        {
            var meta = pageTypeId == CategoryPages.CategoryLandingPageTypeId
                ? "ComposerCategoryLandingPage"
                : "ComposerCategoryPage";

            return meta;
        }

        protected virtual BrowsingByCategoryParam BuildParameters(string categoryId, int page, string sortBy, string sortDirection)
        {
            var searchCriteria = new SearchCriteria
            {
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (page - 1) * SearchConfiguration.MaxItemsPerPage,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope
            };

            searchCriteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(Request.QueryString));

            var param = new BrowsingByCategoryParam
            {
                CategoryId = categoryId,
                Criteria = searchCriteria,
                CategoryName = GetCategoryName(categoryId),
                IsAllProducts = IsAllProductPage(),
                InventoryLocationIds = GetInventoryLocationIds()
            };

            return param;
        }

        protected virtual List<string> GetInventoryLocationIds()
        {
            var ids = InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result;
            return ids;
        }

        private string GetCategoryName(string categoryId)
        {
            var categoryViewModels = CategoryViewService.GetCategoriesPathAsync(new GetCategoriesPathParam()
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CategoryId = categoryId

            }).Result;

            if (categoryViewModels == null)
            {
                return string.Empty;
            }

            var category = categoryViewModels.FirstOrDefault(c => string.Equals(c.Id, categoryId, StringComparison.InvariantCultureIgnoreCase));
            
            return category == null ? string.Empty : category.DisplayName;
        }

        private bool IsAllProductPage()
        {
            var currentPage = PageService.GetPage(SitemapNavigator.CurrentPageId);
            
            if (currentPage == null)
            {
                return false;
            }

            var metaDefName = GetMetadataDefinitionName(currentPage.PageTypeId);

            var composerCategoryPage = currentPage.GetMetaData(metaDefName, typeof (CategoryPage)) as CategoryPage;
            return composerCategoryPage != null && composerCategoryPage.IsAllProductsPage;
        }

        public ActionResult LanguageSwitch()
        {
            var languageSwitchViewModel = LanguageSwitchService.GetViewModel(BuildUrl, ComposerContext.CultureInfo);
            
            return View("LanguageSwitch", languageSwitchViewModel);
        }

        protected virtual string BuildUrl(CultureInfo ci)
        {
            return PageService.GetRendererPageUrl(SitemapNavigator.CurrentPageId, ci);
        }
    }
}
