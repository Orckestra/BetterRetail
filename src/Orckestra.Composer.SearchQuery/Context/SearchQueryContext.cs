using System;
using System.Threading.Tasks;
using System.Web;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.RequestConstants;
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
        protected ISearchUrlProvider SearchUrlProvider { get; }
        protected IBaseSearchCriteriaProvider BaseSearchCriteriaProvider { get; private set; }

        private SearchQueryViewModel _viewModel { get; set; }
        private SearchQueryViewModel _topResultsViewModel { get; set; }
        protected HttpRequestBase Request { get; private set; }
        public virtual string SortBy => Request[SearchRequestParams.SortBy] ?? SearchRequestParams.DefaultSortBy;
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
            ISearchUrlProvider searchUrlProvider,
            HttpRequestBase request,
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchQueryViewService = searchQueryViewService ?? throw new ArgumentNullException(nameof(searchQueryViewService));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider)); ;
        }

        public async Task<SearchQueryViewModel> GetSearchQueryViewModelAsync(SearchQueryType queryType, string queryName)
        {
            if (_viewModel != null && _viewModel.QueryName == queryName && _viewModel.QueryType == queryType) { return _viewModel; }
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

        public async Task<SearchQueryViewModel> GetTopSearchQueryViewModelAsync(string queryTypeValue, string queryName, int pageSize)
        {
            Enum.TryParse(queryTypeValue, out SearchQueryType queryType);
            return await GetTopSearchQueryViewModelAsync(queryType, queryName, pageSize).ConfigureAwait(false);
        }

        public async Task<SearchQueryViewModel> GetTopSearchQueryViewModelAsync(SearchQueryType queryType, string queryName, int pageSize)
        {
            if (_topResultsViewModel != null && _topResultsViewModel.QueryName == queryName && _topResultsViewModel.QueryType == queryType) { return _topResultsViewModel; }

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
            var criteria = BaseSearchCriteriaProvider.GetSearchCriteriaAsync(null, RequestUtils.GetBaseUrl(Request).ToString(), true, CurrentPage).Result;
            criteria.SortBy = SortBy;
            criteria.SortDirection = SortDirection;

            criteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(Request.QueryString));
            return criteria;
        }
    }
}