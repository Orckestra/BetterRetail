using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.Extensions;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.SearchQuery.Providers;
using Orckestra.Composer.SearchQuery.Repositories;
using Orckestra.Composer.SearchQuery.Services;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IInventoryRepository = Orckestra.Composer.SearchQuery.Repositories.IInventoryRepository;

namespace Orckestra.Composer.Grocery.Services
{
    public class GrocerySearchQueryViewService : SearchQueryViewService
    {
        protected ILookupService LookupService { get; }
        public GrocerySearchQueryViewService(
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
            ISearchQueryRepository searchQueryRepository,
            ISearchQueryUrlProvider searchQueryUrlProvider,
            IProductSettingsRepository productSettingsRepository,
            IInventoryRepository inventoryRepository,
            IRecurringOrdersSettings recurringOrdersSettings,
            ILookupService lookupService)
            : base(searchRepository,
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
                searchQueryRepository,
                searchQueryUrlProvider,
                productSettingsRepository,
                inventoryRepository,
                recurringOrdersSettings)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        protected override async Task<ProductSearchViewModel> CreateProductSearchViewModelAsync(ProductDocument productDocument, CreateProductSearchResultsViewModelParam<SearchParam> createSearchViewModelParam, IDictionary<Tuple<string, string>, ProductMainImage> imgDictionary)
        {
            var productSearchViewModel =
                await base.CreateProductSearchViewModelAsync(productDocument, createSearchViewModelParam, imgDictionary);

            await productSearchViewModel.BuildProductBadgeValues<SearchParam>(createSearchViewModelParam, LookupService, createSearchViewModelParam.SearchParam.Criteria.CultureInfo.Name);
            return productSearchViewModel;
        }
    }
}
