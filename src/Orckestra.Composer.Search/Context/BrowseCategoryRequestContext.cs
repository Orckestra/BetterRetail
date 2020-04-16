using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Search.Context
{
    public class BrowseCategoryRequestContext : IBrowseCategoryRequestContext
    {
        protected IComposerContext ComposerContext { get; }
        protected ICategoryBrowsingViewService CategoryBrowsingViewService { get; }
        protected ISearchUrlProvider SearchUrlProvider { get; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; }
        protected ICategoryViewService CategoryViewService { get; }
        protected ICategoryMetaContext CategoryMetaContext { get; }

        protected CategoryBrowsingViewModel ViewModel { get; set; }

        public BrowseCategoryRequestContext(IComposerContext composerContext, ICategoryBrowsingViewService categoryBrowsingViewService,
            ISearchUrlProvider searchUrlProvider, IInventoryLocationProvider inventoryLocationProvider, ICategoryViewService categoryViewService,
            ICategoryMetaContext categoryMetaContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CategoryBrowsingViewService = categoryBrowsingViewService ?? throw new ArgumentNullException(nameof(categoryBrowsingViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));
            CategoryViewService = categoryViewService ?? throw new ArgumentNullException(nameof(categoryViewService));
            CategoryMetaContext = categoryMetaContext;
        }

        public async Task<CategoryBrowsingViewModel> GetCategoryAvailableProductsAsync(GetBrowseCategoryParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            if (ViewModel != null)
            {
                return ViewModel;
            }

            ViewModel = await CategoryBrowsingViewService.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = param.CategoryId,
                CategoryName = await GetCategoryNameAsync(param.CategoryId),
                BaseUrl = RequestUtils.GetBaseUrl(param.Request).ToString(),
                IsAllProducts = CategoryMetaContext.GetIsAllProductPage(),
                Page = param.Page,
                SortBy = param.SortBy,
                SortDirection = param.SortDirection,
                InventoryLocationIds = await InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync(),
                SelectedFacets = SearchUrlProvider.BuildSelectedFacets(param.Request.QueryString).ToList(),
                CultureInfo = ComposerContext.CultureInfo,
            }).ConfigureAwait(false);

            return ViewModel;
        }

        private async Task<string> GetCategoryNameAsync(string categoryId)
        {
            var categoryViewModels = await CategoryViewService.GetCategoriesPathAsync(new GetCategoriesPathParam()
            {
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                CategoryId = categoryId

            });

            if (categoryViewModels == null)
            {
                return string.Empty;
            }

            var category = categoryViewModels.FirstOrDefault(c => string.Equals(c.Id, categoryId, StringComparison.InvariantCultureIgnoreCase));

            return category == null ? string.Empty : category.DisplayName;
        }
    }
}
