using System;
using System.Web;
using System.Web.Http;
using Composite.Core;
using Orckestra.Composer.ContentSearch.Parameters;
using Orckestra.Composer.ContentSearch.Request;
using Orckestra.Composer.ContentSearch.Services;
using Orckestra.Composer.ContentSearch.ViewModels;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.RequestConstants;
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
        protected IContentSearchViewService ContentSearchViewService { get; private set; }

        public ContentSearchController(
            IComposerContext composerContext,
            IContentSearchViewService contentSearchViewService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
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
                CurrentSiteOnly = request.IsCurrentSiteOnly,
                SearchQuery = Keywords,
                IsCorrectedSearchQuery = false,
                CorrectedSearchQuery = string.Empty,
                BaseUrl = BaseUrl,
                QueryKeys = queryString?.AllKeys,
                ProductsTabActive = false,
                SortBy = SortBy,
                SortDirection = SortDirection
            });

            return Ok(contentSearchVm);
        }
    }
}