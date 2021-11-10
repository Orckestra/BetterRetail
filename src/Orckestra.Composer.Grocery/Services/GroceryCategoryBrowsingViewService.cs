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
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryCategoryBrowsingViewService : CategoryBrowsingViewService
    {
        protected ILookupService LookupService { get; }
        public GroceryCategoryBrowsingViewService(
            ISearchRepository searchRepository,
            IViewModelMapper viewModelMapper,
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            IProductUrlProvider productUrlProvider,
            ISearchUrlProvider searchUrlProvider,
            ICategoryRepository categoryRepository,
            ICategoryBrowsingUrlProvider categoryBrowsingUrlProvider,
            IFacetFactory facetFactory,
            ISelectedFacetFactory selectedFacetFactory,
            IPriceProvider priceProvider,
            IComposerContext composerContext,
            IProductSettingsViewService productSettings,
            IScopeViewService scopeViewService,
            IRecurringOrdersSettings recurringOrdersSettings,
            IFulfillmentContext fulfillmentContext,
            ILookupService lookupService)
            : base(searchRepository,
                viewModelMapper,
                damProvider,
                localizationProvider,
                productUrlProvider,
                searchUrlProvider,
                categoryRepository,
                categoryBrowsingUrlProvider,
                facetFactory,
                selectedFacetFactory,
                priceProvider,
                composerContext,
                productSettings,
                scopeViewService,
                recurringOrdersSettings,
                fulfillmentContext)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        protected override async Task<ProductSearchViewModel> CreateProductSearchViewModelAsync(ProductDocument productDocument, CreateProductSearchResultsViewModelParam<BrowsingSearchParam> createSearchViewModelParam, IDictionary<Tuple<string, string>, ProductMainImage> imgDictionary)
        {
            var productSearchViewModel =
                await base.CreateProductSearchViewModelAsync(productDocument, createSearchViewModelParam, imgDictionary);

            await productSearchViewModel.BuildProductBadgeValues<BrowsingSearchParam>(createSearchViewModelParam, LookupService, createSearchViewModelParam.SearchParam.Criteria.CultureInfo.Name);
            return productSearchViewModel;
        }
    }
}
