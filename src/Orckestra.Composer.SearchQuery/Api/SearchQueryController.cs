using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.Requests;
using Orckestra.Composer.SearchQuery.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Orckestra.Composer.SearchQuery.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class SearchQueryController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchQueryViewService SearchQueryViewService { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; set; }
        protected IBaseSearchCriteriaProvider BaseSearchCriteriaProvider { get; private set; }

        public SearchQueryController(
            IComposerContext composerContext,
            ISearchQueryViewService searchQueryViewService,
            ISearchUrlProvider searchUrlProvider,
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchQueryViewService = searchQueryViewService ?? throw new ArgumentNullException(nameof(searchQueryViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider));
        }


        [ActionName("getqueryfacets")]
        [HttpPost]
        [ValidateModelState]
        public virtual async Task<IHttpActionResult> GetFacets(GetQueryFacetsRequest request)
        {
            var queryString = HttpUtility.ParseQueryString(request.QueryString);

            var criteria = await BaseSearchCriteriaProvider.GetSearchCriteriaAsync(null, RequestUtils.GetBaseUrl(Request).ToString(), true).ConfigureAwait(false);
            criteria.NumberOfItemsPerPage = 0;

            criteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(queryString));

            var param = new GetSearchQueryViewModelParams
            {
                QueryName = request.QueryName,
                QueryType = request.QueryType,
                Scope = ComposerContext.Scope,
                CultureInfo = ComposerContext.CultureInfo,
                Criteria = criteria,
                InventoryLocationIds = criteria.InventoryLocationIds
            };

            var viewModel = await SearchQueryViewService.GetSearchQueryViewModelAsync(param).ConfigureAwait(false);
            viewModel.ProductSearchResults.Facets = viewModel.ProductSearchResults.Facets.Where(f => !f.FieldName.StartsWith(SearchConfiguration.CategoryFacetFiledNamePrefix)).ToList();

            return Ok(viewModel);
        }
    }
}