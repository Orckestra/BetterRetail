using System;
using System.Threading.Tasks;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Search.Context
{
    public class BrowseCategoryRequestContext : IBrowseCategoryRequestContext
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ICategoryBrowsingViewService CategoryBrowsingViewService { get; private set; }

        protected CategoryBrowsingViewModel ViewModel { get; set; }

        public BrowseCategoryRequestContext(IComposerContext composerContext, ICategoryBrowsingViewService categoryBrowsingViewService)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (categoryBrowsingViewService == null) { throw new ArgumentNullException("categoryBrowsingViewService"); }

            ComposerContext = composerContext;
            CategoryBrowsingViewService = categoryBrowsingViewService;
        }

        public async Task<CategoryBrowsingViewModel> GetCategoryAvailableProductsAsync(BrowsingByCategoryParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            if (ViewModel != null)
            {
                return ViewModel;
            }

            param.Criteria.Scope = ComposerContext.Scope;
            param.Criteria.CultureInfo = ComposerContext.CultureInfo;

            ViewModel = await CategoryBrowsingViewService.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = param.CategoryId,
                CategoryName = param.CategoryName,
                BaseUrl = param.Criteria.BaseUrl,
                IsAllProducts = param.IsAllProducts,
                Page = param.Criteria.Page,
                SortBy = param.Criteria.SortBy,
                SortDirection = param.Criteria.SortDirection,
                InventoryLocationIds = param.InventoryLocationIds,
                SelectedFacets = param.Criteria.SelectedFacets,
                CultureInfo = param.Criteria.CultureInfo
            }).ConfigureAwait(false);

            return ViewModel;
        }
    }
}
