using Composite.Data;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.RequestConstants;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public class BrowsingCategoriesBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IBrowseCategoryRequestContext RequestContext { get; private set; }
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IPageService PageService { get; private set; }
        protected ICategoryMetaContext CategoryMetaContext { get; }

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
            IBrowseCategoryRequestContext requestContext,
            ILanguageSwitchService languageSwitchService,
            IPageService pageService,
            ICategoryMetaContext categoryMetaContext)
        {
            ComposerContext = composerContext;
            RequestContext = requestContext;
            LanguageSwitchService = languageSwitchService;
            PageService = pageService;
            CategoryMetaContext = categoryMetaContext;
        }

        public virtual ActionResult Summary(
            [Bind(Prefix = SearchRequestParams.Page)]int page = 1,
            [Bind(Prefix = SearchRequestParams.SortBy)]string sortBy = null,
            [Bind(Prefix = SearchRequestParams.SortDirection)]string sortDirection = null)
        {
            return ExecuteBrowsing("CategoryBrowsingSummaryEmpty", "CategoryBrowsingSummary", c => c, null, page, sortBy, sortDirection);
        }

        public virtual ActionResult Index(
            [Bind(Prefix = SearchRequestParams.Page)]int page = 1,
            [Bind(Prefix = SearchRequestParams.SortBy)]string sortBy = null,
            [Bind(Prefix = SearchRequestParams.SortDirection)]string sortDirection = null)
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
            var categoryId = CategoryMetaContext.GetCategoryId();
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return View(emptyView, emptyViewModel);
            }

            var container = RequestContext.GetCategoryAvailableProductsAsync(new GetBrowseCategoryParam
            {
                Request = Request,
                Page = page,
                SortBy = sortBy,
                SortDirection = sortDirection,
                CategoryId = categoryId,
            }).Result;

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

        protected string GetMetadataDefinitionName(Guid pageTypeId)
        {
            var meta = pageTypeId == CategoryPages.CategoryLandingPageTypeId
                ? "ComposerCategoryLandingPage"
                : "ComposerCategoryPage";

            return meta;
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
