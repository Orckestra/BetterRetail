using System;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public class SearchRequestContext : ISearchRequestContext
    {
        protected IComposerRequestContext ComposerContext { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected SearchViewModel SearchViewModel { get; private set; }
        public IInventoryLocationProvider InventoryLocationProvider { get; set; }
        protected ISearchUrlProvider SearchUrlProvider { get; private set; }

        public SearchRequestContext(IComposerRequestContext composerContext, 
            ISearchViewService searchViewService, 
            IInventoryLocationProvider inventoryLocationProvider,
            ISearchUrlProvider searchUrlProvider)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (searchViewService == null) { throw new ArgumentNullException("searchViewService"); }
            if (inventoryLocationProvider == null) { throw new ArgumentNullException(nameof(inventoryLocationProvider)); }
            if (searchUrlProvider == null) { throw new ArgumentNullException(nameof(searchUrlProvider)); }

            ComposerContext = composerContext;
            SearchViewService = searchViewService;
            InventoryLocationProvider = inventoryLocationProvider;
            SearchUrlProvider = searchUrlProvider;
        }

        public virtual async Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            return await SearchViewService.GetPageHeaderViewModelAsync(param).ConfigureAwait(false);
        }

        public virtual async Task<SearchViewModel> GetSearchViewModelAsync(GetSearchViewModelParam param)
        {
            if (SearchViewModel != null)
            {
                return SearchViewModel;
            }

            var criteria = BuildSearchCriteria(param);

            SearchViewModel = await SearchViewService.GetSearchViewModelAsync(criteria).ConfigureAwait(false);

            return SearchViewModel;
        }

        protected virtual SearchCriteria BuildSearchCriteria(GetSearchViewModelParam param)
        {
            var criteria = new SearchCriteria
            {
                Keywords = param.Keywords,
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (param.Page - 1) * SearchConfiguration.MaxItemsPerPage,
                SortBy = param.SortBy,
                SortDirection = param.SortDirection,
                Page = param.Page,
                BaseUrl = RequestUtils.GetBaseUrl(param.Request).ToString(),
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                InventoryLocationIds = InventoryLocationProvider.GetInventoryLocationIdsForSearchAsync().Result,
                AutoCorrect = SearchConfiguration.AutoCorrectSearchTerms
            };
            criteria.SelectedFacets.AddRange(SearchUrlProvider.BuildSelectedFacets(param.Request.QueryString));
            return criteria;
        }
    }
}