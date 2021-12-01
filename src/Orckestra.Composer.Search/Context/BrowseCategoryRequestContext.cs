using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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

        private CategoryBrowsingViewModel _viewModel;

        protected HttpRequestBase Request { get; private set; }

        public virtual string SortBy => Request[SearchRequestParams.SortBy] ?? SearchConfiguration.DefaultSortBy;
        public virtual string SortDirection => Request[SearchRequestParams.SortDirection] ?? SearchConfiguration.DefaultSortDirection;

        public virtual int CurrentPage
        {
            get
            {
                return int.TryParse(Request[SearchRequestParams.Page], out int page) && page > 0 ? page : 1;
            }
        }

        public BrowseCategoryRequestContext(
            IComposerContext composerContext,
            ICategoryBrowsingViewService categoryBrowsingViewService,
            ISearchUrlProvider searchUrlProvider,
            IInventoryLocationProvider inventoryLocationProvider,
            ICategoryViewService categoryViewService,
            ICategoryMetaContext categoryMetaContext,
            HttpRequestBase request)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            CategoryBrowsingViewService = categoryBrowsingViewService ?? throw new ArgumentNullException(nameof(categoryBrowsingViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));
            CategoryViewService = categoryViewService ?? throw new ArgumentNullException(nameof(categoryViewService));
            CategoryMetaContext = categoryMetaContext ?? throw new ArgumentNullException(nameof(categoryMetaContext));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public virtual async Task<CategoryBrowsingViewModel> GetViewModelAsync()
        {
            if (_viewModel != null)
            {
                return _viewModel;
            }
            var categoryId = CategoryMetaContext.GetCategoryId();
            _viewModel = await CategoryBrowsingViewService.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
            {
                CategoryId = categoryId,
                CategoryName = await GetCategoryNameAsync(categoryId).ConfigureAwait(false),
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                IsAllProducts = CategoryMetaContext.GetIsAllProductPage(),
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                Page = CurrentPage,
                SortBy = SortBy,
                SortDirection = SortDirection,
                InventoryLocationIds = await InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().ConfigureAwait(false),
                SelectedFacets = SearchUrlProvider.BuildSelectedFacets(Request.QueryString).ToList(),
                CultureInfo = ComposerContext.CultureInfo,
            }).ConfigureAwait(false);

            return _viewModel;

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
