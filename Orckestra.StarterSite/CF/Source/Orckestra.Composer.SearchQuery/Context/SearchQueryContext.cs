using Orckestra.Composer.Search.Services;
using Orckestra.Composer.SearchQuery.Parameters;
using Orckestra.Composer.SearchQuery.Services;
using Orckestra.Composer.SearchQuery.ViewModels;
using Orckestra.Composer.Services;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.SearchQuery.Context
{
    public class SearchQueryContext : ISearchQueryContext
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchQueryViewService SearchQueryViewService { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }

        protected SearchQueryViewModel ViewModel { get; set; }

        public SearchQueryContext(IComposerContext composerContext, ISearchQueryViewService searchQueryViewService, ISearchViewService searchViewService)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (searchQueryViewService == null) { throw new ArgumentNullException("searchQueryViewService"); }
            if (searchViewService == null) { throw new ArgumentNullException("searchViewService"); }

            ComposerContext = composerContext;
            SearchQueryViewService = searchQueryViewService;
            SearchViewService = searchViewService;
        }

        public async Task<SearchQueryViewModel> GetSearchQueryViewModelAsync(GetSearchQueryViewModelParams param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            if (ViewModel != null)
            {
                return ViewModel;
            }

            param.Scope = ComposerContext.Scope;
            param.CultureInfo = ComposerContext.CultureInfo;

            ViewModel = await SearchQueryViewService.GetSearchQueryViewModelAsync(param).ConfigureAwait(false);

            return ViewModel;
        }
    }
}
