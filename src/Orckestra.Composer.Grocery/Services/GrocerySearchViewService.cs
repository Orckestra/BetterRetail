using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.ViewModels;
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
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Services.Lookup;

namespace Orckestra.Composer.Grocery.Services
{
    public class GrocerySearchViewService : SearchViewService
    {
        protected ILookupService LookupService { get; }
        public GrocerySearchViewService(ICategoryRepository categoryRepository,
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
            IRecurringOrdersSettings recurringOrdersSettings,
            ILookupService lookupService)
            : base(categoryRepository,
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
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        }

        protected override async Task<ProductSearchViewModel> CreateProductSearchViewModelAsync(ProductDocument productDocument, CreateProductSearchResultsViewModelParam<SearchParam> createSearchViewModelParam, IDictionary<Tuple<string, string>, ProductMainImage> imgDictionary)
        {
            var productSearchViewModel =
                await base.CreateProductSearchViewModelAsync(productDocument, createSearchViewModelParam, imgDictionary);

            await BuildProductBadgeValues(productSearchViewModel, createSearchViewModelParam);
            return productSearchViewModel;
        }

        protected virtual async Task BuildProductBadgeValues(ProductSearchViewModel productSearchViewModel, CreateProductSearchResultsViewModelParam<SearchParam> createSearchViewModelParam)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();
            if (extendedVM.ProductBadges == null)
            { return; }

            IDictionary<string, string> productBadgesLookupValueDictionary = new Dictionary<string, string>();

            var productLookups = await LookupService.GetLookupsAsync(LookupType.Product);
            productLookups
                .FirstOrDefault(item => item.LookupName == "ProductBadges")
                .Values
                .ForEach(item => productBadgesLookupValueDictionary.Add(item.DisplayName.GetLocalizedValue(createSearchViewModelParam.SearchParam.Criteria.CultureInfo.Name), item.Value));

            extendedVM.ProductBadgeValues = new Dictionary<string, string>();
            foreach (var extendedVmProductBadge in extendedVM.ProductBadges)
            {
                if (productBadgesLookupValueDictionary.ContainsKey(extendedVmProductBadge))
                    extendedVM.ProductBadgeValues.Add(productBadgesLookupValueDictionary[extendedVmProductBadge], extendedVmProductBadge);
            }

            productSearchViewModel.Context["ProductBadgeValues"] = extendedVM.ProductBadgeValues;
        }
    }
}
