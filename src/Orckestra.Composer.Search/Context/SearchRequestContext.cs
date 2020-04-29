using System;
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
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public class SearchRequestContext : ISearchRequestContext
    {
        private readonly Lazy<SearchViewModel> _viewModel;

        protected IComposerContext ComposerContext { get; private set; }
        protected HttpRequestBase Request { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected SearchViewModel SearchViewModel { get; private set; }
        public IInventoryLocationProvider InventoryLocationProvider { get; set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }

        public SearchRequestContext(IComposerContext composerContext,
            ISearchViewService searchViewService,
            IInventoryLocationProvider inventoryLocationProvider,
            ISearchUrlProvider searchUrlProvider,
            HttpRequestBase request)
        {

            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            Request = request;

            _viewModel = new Lazy<SearchViewModel>(() =>
            {
                var criteria = BuildProductsSearchCriteria();
                return SearchViewService.GetSearchViewModelAsync(criteria).Result;
            });
        }


        public bool IsProductsSearchActive { get; set; }
        public virtual int CurrentPage
        {
            get
            {
                return int.TryParse(Request[SearchRequestParams.Page], out int page) && page > 0 ? page : 1;
            }
        }

        public virtual string SearchQuery
        {
            get
            {
                return Request[SearchRequestParams.Keywords];
            }
        }

        public virtual string SortBy
        {
            get
            {
                return Request[SearchRequestParams.SortBy];
            }
        }

        public virtual string SortDirection
        {
            get
            {
                return Request[SearchRequestParams.SortDirection] ?? SearchRequestParams.DefaultSortDirection;
            }
        }

        public virtual SearchViewModel ProductsSearchViewModel =>  _viewModel.Value;

        public virtual async Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            return await SearchViewService.GetPageHeaderViewModelAsync(param).ConfigureAwait(false);
        }

        protected virtual SearchCriteria BuildProductsSearchCriteria()
        {
            var criteria = new SearchCriteria
            {
                Keywords = SearchQuery,
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (CurrentPage - 1) * SearchConfiguration.MaxItemsPerPage,
                SortBy = IsProductsSearchActive ?  SortBy: null,
                SortDirection = IsProductsSearchActive ?  SortDirection: null,
                Page = CurrentPage,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                InventoryLocationIds = InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result,
                AutoCorrect = SearchConfiguration.AutoCorrectSearchTerms
            };
            criteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(Request.QueryString));
            return criteria;
        }
    }
}