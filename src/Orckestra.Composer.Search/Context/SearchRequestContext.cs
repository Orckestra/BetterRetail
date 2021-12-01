using System;
using System.Threading.Tasks;
using System.Web;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public class SearchRequestContext : ISearchRequestContext
    {
        private readonly Lazy<SearchViewModel> _viewModel;

        protected HttpRequestBase Request { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }
        protected IBaseSearchCriteriaProvider BaseSearchCriteriaProvider { get; private set; }

        public SearchRequestContext(
            ISearchViewService searchViewService,
            ISearchUrlProvider searchUrlProvider,
            HttpRequestBase request,
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider)
        {
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            Request = request;
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider));

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
                return Request[SearchRequestParams.SortBy] ?? SearchConfiguration.DefaultSortBy;
            }
        }

        public virtual string SortDirection
        {
            get
            {
                return Request[SearchRequestParams.SortDirection] ?? SearchConfiguration.DefaultSortDirection;
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
            var criteria = BaseSearchCriteriaProvider.GetSearchCriteriaAsync(
                SearchQuery, 
                SearchConfiguration.MaxItemsPerPage, 
                (CurrentPage - 1) * SearchConfiguration.MaxItemsPerPage, 
                RequestUtils.GetBaseUrl(Request).ToString(), 
                true,
                CurrentPage,
                IsProductsSearchActive ? SortBy : null,
                IsProductsSearchActive ? SortDirection : null).Result;
           
            criteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(Request.QueryString));
            return criteria;
        }
    }
}