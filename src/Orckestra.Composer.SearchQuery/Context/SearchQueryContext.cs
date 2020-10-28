using System;
using System.Threading.Tasks;
using System.Web;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.Services;
using Orckestra.Composer.SearchQuery.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.SearchQueries;

namespace Orckestra.Composer.SearchQuery.Context
{
    public class SearchQueryContext : ISearchQueryContext
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchQueryViewService SearchQueryViewService { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected IInventoryLocationProvider InventoryLocationProvider { get; }
        protected ISearchUrlProvider SearchUrlProvider { get; }

        private SearchQueryViewModel _viewModel { get; set; }
        private SearchQueryViewModel _topResultsViewModel { get; set; }
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

        public SearchQueryContext(IComposerContext composerContext,
            ISearchQueryViewService searchQueryViewService,
            ISearchViewService searchViewService,
            IInventoryLocationProvider inventoryLocationProvider,
            ISearchUrlProvider searchUrlProvider,
            HttpRequestBase request)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchQueryViewService = searchQueryViewService ?? throw new ArgumentNullException(nameof(searchQueryViewService));
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            InventoryLocationProvider = inventoryLocationProvider ?? throw new ArgumentNullException(nameof(inventoryLocationProvider));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
        }

        public async Task<SearchQueryViewModel> GetSearchQueryViewModelAsync(SearchQueryType queryType, string queryName)
        {
            if (_viewModel != null) { return _viewModel; }
            var criteria = BuildProductsSearchCriteria();
            var param = new GetSearchQueryViewModelParams
            {
                QueryName = queryName,
                QueryType = queryType,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                Criteria = criteria,
                InventoryLocationIds = criteria.InventoryLocationIds
            };

            _viewModel = await SearchQueryViewService.GetSearchQueryViewModelAsync(param).ConfigureAwait(false);

            return _viewModel;
        }

        public async Task<SearchQueryViewModel> GetTopSearchQueryViewModelAsync(SearchQueryType queryType, string queryName, int pageSize)
        {
            if (_topResultsViewModel != null) { return _topResultsViewModel; }

            var criteria = BuildProductsSearchCriteria();
            criteria.NumberOfItemsPerPage = pageSize;
            var param = new GetSearchQueryViewModelParams
            {
                QueryName = queryName,
                QueryType = queryType,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                Criteria = criteria,
                InventoryLocationIds = criteria.InventoryLocationIds
            };

            _topResultsViewModel = await SearchQueryViewService.GetSearchQueryViewModelAsync(param).ConfigureAwait(false);
            if (_topResultsViewModel.ProductSearchResults.Pagination != null)
            {
                _topResultsViewModel.ProductSearchResults.Pagination.TotalNumberOfPages = 1;
                _topResultsViewModel.ProductSearchResults.Pagination.NextPage = null;
            }

            return _topResultsViewModel;
        }

        protected virtual SearchCriteria BuildProductsSearchCriteria()
        {
            var criteria = new SearchCriteria
            {
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (CurrentPage - 1) * SearchConfiguration.MaxItemsPerPage,
                SortBy = SortBy,
                SortDirection = SortDirection,
                Page = CurrentPage,
                BaseUrl = RequestUtils.GetBaseUrl(Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                InventoryLocationIds = InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result
            };
            criteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(Request.QueryString));
            return criteria;
        }
    }
}