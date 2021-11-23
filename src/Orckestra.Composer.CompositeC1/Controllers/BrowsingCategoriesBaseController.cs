using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Context;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1.Controllers
{
    public class BrowsingCategoriesBaseController : Controller
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected IBrowseCategoryRequestContext RequestContext { get; private set; }
        protected IPageService PageService { get; private set; }
        protected ICategoryMetaContext CategoryMetaContext { get; }

        protected static CategoryBrowsingViewModel EmptyCategoryBrowsingContainer { get; set; }

        static BrowsingCategoriesBaseController()
        {
            EmptyCategoryBrowsingContainer = new CategoryBrowsingViewModel
            {
                ProductSearchResults = new ProductSearchResultsViewModel { Keywords = string.Empty }
            };
        }

        public BrowsingCategoriesBaseController(
            IComposerContext composerContext,
            IBrowseCategoryRequestContext requestContext,
            IPageService pageService,
            ICategoryMetaContext categoryMetaContext)
        {
            ComposerContext = composerContext;
            RequestContext = requestContext;
            PageService = pageService;
            CategoryMetaContext = categoryMetaContext;
        }

        public virtual ActionResult Index(
            [Bind(Prefix = SearchRequestParams.Page)]int page = 1,
            [Bind(Prefix = SearchRequestParams.SortBy)]string sortBy = null,
            [Bind(Prefix = SearchRequestParams.SortDirection)]string sortDirection = null)
        {
            return ExecuteBrowsing("ProductsSearchResults", "ProductsSearchResults", c => c, EmptyCategoryBrowsingContainer, page, sortBy, sortDirection);
        }


        public virtual ActionResult ChildCategories(int page = 1, string sortBy = null, string sortDirection = null)
        {
            return ExecuteBrowsing("ChildCategories", "ChildCategories", c => c, EmptyCategoryBrowsingContainer, page, sortBy, sortDirection);
        }

        protected ActionResult ExecuteBrowsing(string emptyView, string filledView, Func<CategoryBrowsingViewModel, object> viewModelSelector, object emptyViewModel, int page, string sortBy = null, string sortDirection = null)
        {
            var categoryId = CategoryMetaContext.GetCategoryId();
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return View(emptyView, emptyViewModel);
            }

            var container = RequestContext.GetViewModelAsync().Result;

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
            if (viewName == "ProductsSearchResults")
            {
                model.Context["SearchResults"] = model.ProductSearchResults.SearchResults;
                model.Context["Keywords"] = model.ProductSearchResults.Keywords;
                model.Context["TotalCount"] = model.ProductSearchResults.TotalCount;
                model.Context["MaxItemsPerPage"] = SearchConfiguration.MaxItemsPerPage;
                model.Context["ListName"] = "Category Browsing";
                model.Context["PaginationCurrentPage"] = model.ProductSearchResults.Pagination.Pages.FirstOrDefault(p => p.IsCurrentPage);
            }
        }
    }
}
