using System;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.Context
{
    public class SearchRequestContext : ISearchRequestContext
    {
        protected IComposerContext ComposerContext { get; private set; }
        protected ISearchViewService SearchViewService { get; private set; }
        protected SearchViewModel SearchViewModel { get; private set; }

        public SearchRequestContext(IComposerContext composerContext, ISearchViewService searchViewService)
        {
            if (composerContext == null) { throw new ArgumentNullException("composerContext"); }
            if (searchViewService == null) { throw new ArgumentNullException("searchViewService"); }

            ComposerContext = composerContext;
            SearchViewService = searchViewService;
        }

        public virtual async Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }

            return await SearchViewService.GetPageHeaderViewModelAsync(param).ConfigureAwait(false);
        }

        public virtual async Task<SearchViewModel> GetSearchViewModelAsync(SearchCriteria criteria)
        {
            if (SearchViewModel != null)
            {
                return SearchViewModel;
            }

            criteria.Scope = ComposerContext.Scope;
            criteria.CultureInfo = ComposerContext.CultureInfo;

            SearchViewModel = await SearchViewService.GetSearchViewModelAsync(criteria).ConfigureAwait(false);

            return SearchViewModel;
        }
    }
}