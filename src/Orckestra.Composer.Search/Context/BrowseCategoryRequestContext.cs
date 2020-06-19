using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.RequestConstants;
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

        public CategoryBrowsingViewModel ViewModel => _viewModel.Value;
        private readonly Lazy<CategoryBrowsingViewModel> _viewModel;

        protected HttpRequestBase Request { get; private set; }

        public virtual string SortBy => Request[SearchRequestParams.SortBy];
        public virtual string SortDirection => Request[SearchRequestParams.SortDirection] ?? SearchRequestParams.DefaultSortDirection;
        
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
            CategoryMetaContext = categoryMetaContext ?? throw new ArgumentNullException(nameof(categoryMetaContext)); ;
            Request = request ?? throw new ArgumentNullException(nameof(request)); ;

            _viewModel = new Lazy<CategoryBrowsingViewModel>(() =>
            {
                var categoryId = CategoryMetaContext.GetCategoryId();
                return CategoryBrowsingViewService.GetCategoryBrowsingViewModelAsync(new GetCategoryBrowsingViewModelParam
                {
                    CategoryId = categoryId,
                    CategoryName = GetCategoryNameAsync(categoryId).Result,
                    BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                    IsAllProducts = CategoryMetaContext.GetIsAllProductPage(),
                    Page = CurrentPage,
                    SortBy = SortBy,
                    SortDirection = SortDirection,
                    InventoryLocationIds = InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result,
                    SelectedFacets = SearchUrlProvider.BuildSelectedFacets(Request.QueryString).ToList(),
                    CultureInfo = ComposerContext.CultureInfo,
                }).Result;
            });
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
