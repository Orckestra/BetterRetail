using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Request;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;

namespace Orckestra.Composer.Grocery.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    [ValidateModelState]
    public class MyUsualsController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; set; }
        protected IBaseSearchCriteriaProvider BaseSearchCriteriaProvider { get; private set; }
        protected IMyUsualsViewService MyUsualsViewService { get; private set; }
        public IScopeProvider ScopeProvider { get; private set; }

        public MyUsualsController(
            IComposerContext composerContext,
            ISearchViewService searchViewService,
            ISearchUrlProvider searchUrlProvider,
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider,
            IMyUsualsViewService myUsualsViewService,
            IScopeProvider scopeProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider));
            MyUsualsViewService = myUsualsViewService ?? throw new ArgumentNullException(nameof(myUsualsViewService));
            ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }


        [ActionName("search")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetSearchResultsMyUsuals(GetSearchResultsRequest request)
        {
            var listSkus = await MyUsualsViewService.GetMyUsualsProductSkusAsync(new GetCustomerOrderedProductsParam
            {
                ScopeId = ScopeProvider.DefaultScope,
                CustomerId = ComposerContext.CustomerId
            }).ConfigureAwait(false);

            SearchViewModel viewModel;
            if (listSkus.Length > 0)
            {
                var queryString = HttpUtility.ParseQueryString(request.QueryString ?? "");
                var SelectedFacets = SearchUrlProvider.BuildSelectedFacets(queryString).ToList();
                var Keywords = queryString[SearchRequestParams.Keywords];
                var BaseUrl = RequestUtils.GetBaseUrl(Request).ToString();
                var IncludeFactes = true;

                var searchCriteria = await BaseSearchCriteriaProvider.GetSearchCriteriaAsync(Keywords, BaseUrl, IncludeFactes).ConfigureAwait(false);
                var searchBySkusCriteria = new SearchBySkusCriteria
                {
                    Skus = listSkus,
                    Keywords = searchCriteria.Keywords,
                    NumberOfItemsPerPage = listSkus.Length,
                    StartingIndex = searchCriteria.StartingIndex,
                    Page = searchCriteria.Page,
                    BaseUrl = searchCriteria.BaseUrl,
                    Scope = searchCriteria.Scope,
                    CultureInfo = searchCriteria.CultureInfo,
                    InventoryLocationIds = searchCriteria.InventoryLocationIds,
                    AvailabilityDate = searchCriteria.AvailabilityDate,
                    IncludeFacets = searchCriteria.IncludeFacets
                };

                searchBySkusCriteria.SelectedFacets.AddRange(SelectedFacets);

                viewModel = await SearchViewService.GetSearchViewModelAsync(searchBySkusCriteria).ConfigureAwait(false);

                if (IncludeFactes)
                {
                    viewModel.ProductSearchResults.Facets = viewModel.ProductSearchResults.Facets.Where(f => !f.FieldName.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix)).ToList();
                }
            } 
            else
            {
                viewModel = new SearchViewModel
                {
                    ProductSearchResults = new ProductSearchResultsViewModel
                    {
                        SearchResults = new List<ProductSearchViewModel>(),
                        TotalCount = 0
                    }
                };
            }

            return Ok(viewModel);
        }
    }
}
