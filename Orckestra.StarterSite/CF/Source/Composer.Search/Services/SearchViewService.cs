using System;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.Repositories;
using System.Collections.Generic;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Search.Services
{
    public class SearchViewService : BaseSearchViewService<SearchParam>, ISearchViewService
    {
        protected ISearchCategoryRepository CategoryRepository { get; }

        public SearchViewService(
            ISearchCategoryRepository categoryRepository,
            ISearchRepository searchRepository,
            IViewModelMapper viewModelMapper,
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            IProductUrlProvider productUrlProvider,
            ISearchUrlProvider searchUrlProvider,
            IFacetFactory facetFactory,
            ISelectedFacetFactory selectedFacetFactory,
            IPriceProvider priceProvider,
            IComposerContext composerContext,
            IProductSettingsViewService productSettings,
            IScopeViewService scopeViewService,
            IRecurringOrdersSettings recurringOrdersSettings)
            : base(
            searchRepository,
            viewModelMapper,
            damProvider,
            localizationProvider,
            productUrlProvider,
            searchUrlProvider,
            facetFactory,
            selectedFacetFactory,
            priceProvider,
            composerContext,
            productSettings,
            scopeViewService,
            recurringOrdersSettings)
        {
            CategoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public virtual Task<PageHeaderViewModel> GetPageHeaderViewModelAsync(GetPageHeaderParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }


            var pageHeaderViewModel = new PageHeaderViewModel
            {
                PageTitle = GetPageTitle(param),
                NoIndex = !IsPageIndexed(param)
            };

            return Task.FromResult(pageHeaderViewModel);
        }

        protected virtual string GetPageTitle(GetPageHeaderParam param)
        {
            var templateTitle = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category = "List-Search",
                Key = "L_SearchResultsForBreadcrumb",
                CultureInfo = ComposerContext.CultureInfo
            });

            var pageTitle = templateTitle + " \"" + param.Keywords + "\"";

            return pageTitle;
        }

        protected virtual bool IsPageIndexed(GetPageHeaderParam param)
        {
            return param.IsPageIndexed;
        }

        public virtual async Task<SearchViewModel> GetSearchViewModelAsync(SearchCriteria criteria)
        {
            if (criteria == null) { throw new ArgumentNullException(nameof(criteria)); }
            if (criteria.SelectedFacets == null) { throw new ArgumentNullException(nameof(criteria.SelectedFacets)); }

            var viewModel = new SearchViewModel
            {
                Keywords = criteria.Keywords,
                SelectedFacets = await GetSelectedFacetsAsync(criteria).ConfigureAwait(false),
                ProductSearchResults = await GetProductSearchResultsAsync(criteria).ConfigureAwait(false)
            };

            return viewModel;
        }

        protected override string GenerateUrl(CreateSearchPaginationParam<SearchParam> param)
        {
            if (param.SearchParameters == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.SearchParameters.Criteria == null) { throw new ArgumentException("param.Criteria is null", nameof(param)); }

            return SearchUrlProvider.BuildSearchUrl(new BuildSearchUrlParam
            {
                SearchCriteria = param.SearchParameters.Criteria,
                CorrectedSearchTerms = param.CorrectedSearchTerms
            });
        }

        protected virtual async Task<ProductSearchResultsViewModel> GetProductSearchResultsAsync(SearchCriteria criteria)
        {
            if (string.IsNullOrWhiteSpace(criteria.Keywords)) { return new ProductSearchResultsViewModel(); }

            var param = new SearchParam
            {
                Criteria = criteria
            };

            return await SearchAsync(param).ConfigureAwait(false);
        }

        protected virtual Task<SelectedFacets> GetSelectedFacetsAsync(SearchCriteria criteria)
        {
            var facets = FlattenFilterList(criteria.SelectedFacets, criteria.CultureInfo);

            return Task.FromResult(facets);
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await CategoryRepository.GetCategoriesAsync(new GetCategoriesParam
            {
                Scope = ComposerContext.Scope
            });
        }

        public async Task<List<Orckestra.Overture.ServiceModel.Search.Facet>> GetCategoryProductCounts(string cultureName)
        {
            return await CategoryRepository.GetCategoryProductCount(ComposerContext.Scope, cultureName);
        }

        public async Task<List<Orckestra.Overture.ServiceModel.Search.Facet>> GetBrandProductCounts(string cultureName)
        {
            return await CategoryRepository.GetBrandProductCount(ComposerContext.Scope, cultureName);
        }
    }
}