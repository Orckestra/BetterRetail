using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Grocery.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orckestra.Composer.Grocery.Context
{
    public class MyUsualsContext : IMyUsualsContext
    {
        private readonly Lazy<SearchViewModel> _emptyProductResultsViewModel;
        private readonly Lazy<string[]> _listMyUsualsSkus;
        private readonly Lazy<SearchViewModel> _productResultsViewModel;

        protected HttpRequestBase Request { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }
        protected IBaseSearchCriteriaProvider BaseSearchCriteriaProvider { get; private set; }
        protected IComposerContext ComposerContext { get; private set; }
        public IScopeProvider ScopeProvider { get; private set; }
        protected IMyUsualsViewService MyUsualsViewService { get; private set; }

        public MyUsualsContext(
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider,
            HttpRequestBase request,
            ISearchViewService searchViewService,
            ISearchUrlProvider searchUrlProvider,
            IComposerContext composerContext,
            IScopeProvider scopeProvider,
            IMyUsualsViewService myUsualsViewService)
        {
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider));
            Request = request;
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            MyUsualsViewService = myUsualsViewService ?? throw new ArgumentNullException(nameof(myUsualsViewService));

            _emptyProductResultsViewModel = new Lazy<SearchViewModel>(() =>
            {
                return new SearchViewModel
                {
                    ProductSearchResults = new ProductSearchResultsViewModel
                    {
                        SearchResults = new List<ProductSearchViewModel>(),
                        Facets = new List<Facet>(),
                        TotalCount = 0
                    }
                };
            });

            _listMyUsualsSkus = new Lazy<string[]>(() =>
            {
                return MyUsualsViewService.GetMyUsualsProductSkusAsync(new GetCustomerOrderedProductsParam
                {
                    ScopeId = ScopeProvider.DefaultScope,
                    CustomerId = ComposerContext.CustomerId
                }).Result;
            });

            _productResultsViewModel = new Lazy<SearchViewModel>(() =>
            {
                if (ListMyUsualsSkus == null || ListMyUsualsSkus.Length == 0)
                {
                    return EmptyProductResultsViewModel;
                }
                var criteria = BuildProductsSearchCriteria();
                var viewModel = SearchViewService.GetSearchViewModelAsync(criteria).Result;
                viewModel.ProductSearchResults.Facets = viewModel.ProductSearchResults.Facets.Where(f => !f.FieldName.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix)).ToList();
                return viewModel;
            });

        }

        public virtual string SearchQuery
        {
            get
            {
                return Request[SearchRequestParams.Keywords];
            }
        }

        public virtual SearchViewModel EmptyProductResultsViewModel => _emptyProductResultsViewModel.Value;

        public virtual SearchViewModel ProductResultsViewModel => _productResultsViewModel.Value;

        public virtual string[] ListMyUsualsSkus => _listMyUsualsSkus.Value;

        public virtual SearchBySkusCriteria BuildProductsSearchCriteria()
        {
            var SelectedFacets = SearchUrlProvider.BuildSelectedFacets(Request.QueryString).ToList();
            var Keywords = Request[SearchRequestParams.Keywords] ?? "*";
            var CurrentPage = int.TryParse(Request[SearchRequestParams.Page], out int page) && page > 0 ? page : 1;
            var SortDirection = Request[SearchRequestParams.SortDirection] ?? SearchRequestParams.DefaultSortDirection;
            var SortBy = Request[SearchRequestParams.SortBy] ?? SearchRequestParams.DefaultSortBy;
            var BaseUrl = RequestUtils.GetBaseUrl(Request).ToString();

            var searchCriteria = BaseSearchCriteriaProvider.GetSearchCriteriaAsync(Keywords, BaseUrl, true, CurrentPage).Result;
            var searchBySkusCriteria = new SearchBySkusCriteria
            {
                Skus = ListMyUsualsSkus,
                Keywords = searchCriteria.Keywords,
                NumberOfItemsPerPage = searchCriteria.NumberOfItemsPerPage,
                StartingIndex = searchCriteria.StartingIndex,
                Page = searchCriteria.Page,
                BaseUrl = searchCriteria.BaseUrl,
                Scope = searchCriteria.Scope,
                CultureInfo = searchCriteria.CultureInfo,
                InventoryLocationIds = searchCriteria.InventoryLocationIds,
                AvailabilityDate = searchCriteria.AvailabilityDate,
                IncludeFacets = searchCriteria.IncludeFacets
            };
            searchBySkusCriteria.SortBy = SortBy;
            searchBySkusCriteria.SortDirection = SortDirection;
            searchBySkusCriteria.SelectedFacets.AddRange(SelectedFacets);

            return searchBySkusCriteria;
        }
    }
}
