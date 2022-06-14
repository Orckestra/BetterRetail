using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Orckestra.Composer.Grocery.Context;
using Orckestra.Composer.Grocery.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Request;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Services;
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
        protected IMyUsualsContext MyUsualsContext { get; }

        public MyUsualsController(
            IComposerContext composerContext,
            ISearchViewService searchViewService,
            ISearchUrlProvider searchUrlProvider,
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider,
            IMyUsualsViewService myUsualsViewService,
            IScopeProvider scopeProvider,
            IMyUsualsContext myUsualsContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider));
            MyUsualsViewService = myUsualsViewService ?? throw new ArgumentNullException(nameof(myUsualsViewService));
            ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            MyUsualsContext = myUsualsContext ?? throw new ArgumentNullException(nameof(myUsualsContext));
        }


        [ActionName("search")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetSearchResultsMyUsuals(GetSearchResultsRequest request)
        {
            var listSkus = MyUsualsContext.ListMyUsualsSkus;

            if (listSkus == null || listSkus.Length == 0)
            {
                return Ok(MyUsualsContext.EmptyProductResultsViewModel);
            }

            var criteria = await MyUsualsViewService.BuildProductsSearchCriteria(listSkus, request.QueryString).ConfigureAwait(false);

            var viewModel = await SearchViewService.GetSearchViewModelAsync(criteria).ConfigureAwait(false);
            viewModel.ProductSearchResults.Facets = viewModel.ProductSearchResults.Facets.Where(f => !f.FieldName.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix)).ToList();

            return Ok(viewModel);
        }


        [ActionName("getfacets")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetFacets(GetFacetsRequest request)
        {
            var listSkus = MyUsualsContext.ListMyUsualsSkus;

            if (listSkus == null || listSkus.Length == 0)
            {
                return Ok(MyUsualsContext.EmptyProductResultsViewModel);
            }

            var criteria = await MyUsualsViewService.BuildProductsSearchCriteria(listSkus, request.QueryString).ConfigureAwait(false);
            criteria.NumberOfItemsPerPage = 0;

            var viewModel = await SearchViewService.GetSearchViewModelAsync(criteria).ConfigureAwait(false);
            viewModel.ProductSearchResults.Facets = viewModel.ProductSearchResults.Facets.Where(f => !f.FieldName.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix)).ToList();

            return Ok(viewModel);
        }
    }
}
