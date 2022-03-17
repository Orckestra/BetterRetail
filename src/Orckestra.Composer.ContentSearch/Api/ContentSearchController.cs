using System;
using System.Web;
using System.Web.Http;
using Orckestra.Composer.ContentSearch.Parameters;
using Orckestra.Composer.ContentSearch.Request;
using Orckestra.Composer.ContentSearch.Services;
using Orckestra.Composer.ContentSearch.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.RequestConstants;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.WebAPIFilters;
using ValidateLanguageAttribute = Orckestra.Composer.WebAPIFilters.ValidateLanguageAttribute;

namespace Orckestra.Composer.ContentSearch.Api
{
    [ValidateLanguage]
    [JQueryOnlyFilter]
    public class ContentSearchController : ApiController
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected ISearchUrlProvider SearchUrlProvider { get; set; }
        protected IBaseSearchCriteriaProvider BaseSearchCriteriaProvider { get; private set; }
        protected IContentSearchViewService ContentSearchViewService { get; private set; }

        public ContentSearchController(
            IComposerContext composerContext,
            ISearchViewService searchViewService,
            ISearchUrlProvider searchUrlProvider,
            IBaseSearchCriteriaProvider baseSearchCriteriaProvider,
            IContentSearchViewService contentSearchViewService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            SearchViewService = searchViewService ?? throw new ArgumentNullException(nameof(searchViewService));
            SearchUrlProvider = searchUrlProvider ?? throw new ArgumentNullException(nameof(searchUrlProvider));
            BaseSearchCriteriaProvider = baseSearchCriteriaProvider ?? throw new ArgumentNullException(nameof(baseSearchCriteriaProvider));
            ContentSearchViewService = contentSearchViewService ?? throw new ArgumentNullException(nameof(contentSearchViewService));
        }

        [ActionName("search")]
        [HttpPost]
        [ValidateModelState]
        public virtual IHttpActionResult GetContentSearchResults(GetContentSearchResultsRequest request)
        {
            var queryString = HttpUtility.ParseQueryString(request.QueryString ?? "");
            var CurrentPage = int.TryParse(queryString[SearchRequestParams.Page], out int page) && page > 0 ? page : 1;
            var SortDirection = queryString[SearchRequestParams.SortDirection] ?? SearchRequestParams.DefaultSortDirection;
            var SortBy = queryString[SearchRequestParams.SortBy] ?? SearchRequestParams.DefaultSortBy;
            var BaseUrl = RequestUtils.GetBaseUrl(Request).ToString();
            var Keywords = queryString[SearchRequestParams.Keywords];

            ContentSearchViewModel contentSearchVm = ContentSearchViewService.GetContentSearchViewModel(new GetContentSearchParameter
            {
                Culture = ComposerContext.CultureInfo,
                PageSize = SearchConfiguration.MaxItemsPerPage,
                CurrentPage = CurrentPage,
                PathInfo = request.CurrentTabPathInfo,
                CurrentSiteOnly = true, //CurrentSite,
                SearchQuery = Keywords,
                IsCorrectedSearchQuery = false,
                CorrectedSearchQuery = string.Empty,
                BaseUrl = BaseUrl, //CurrentPageNode.Url,
                QueryKeys = queryString?.AllKeys,
                ProductsTabActive = false, //isProductTab,
                SortBy = SortBy,
                SortDirection = SortDirection
            });

            return Ok(contentSearchVm);
        }
    }
}