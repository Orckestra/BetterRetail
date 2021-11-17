using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Metadata;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryProductSearchViewModelFactory : ProductSearchViewModelFactory
    {
        protected ILookupService LookupService { get; }
        public IConverterProvider ConverterProvider { get; }

        public GroceryProductSearchViewModelFactory(
            IViewModelMapper viewModelMapper,
            IProductUrlProvider productUrlProvider,
            IRecurringOrdersSettings recurringOrdersSettings,
            IComposerContext composerContext, 
            IProductSettingsViewService productSettings, 
            IPriceProvider priceProvider,
            ILookupService lookupService,
            IConverterProvider converterProvider) 
            : base(
                viewModelMapper,
                productUrlProvider,
                recurringOrdersSettings, 
                composerContext,
                productSettings, 
                priceProvider)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            ConverterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
        }
        
        public override async Task<IList<ProductSearchViewModel>> EnrichAppendProductSearchViewModels(IList<(ProductSearchViewModel, ProductDocument)> productSearchResultList, SearchCriteria criteria)
        {
            var productSearchViewModels = await base.EnrichAppendProductSearchViewModels(productSearchResultList, criteria);
            var productLookups = await LookupService.GetLookupsAsync(LookupType.Product);
            var productBadgesLookupValues = GetLookupValuesByDisplayNameKeys(productLookups, criteria.CultureInfo.Name, "ProductBadges");
            var unitOfMeasureLookupValues = GetLookupValuesByDisplayNameKeys(productLookups, criteria.CultureInfo.Name, "UnitOfMeasure");

            foreach (var productSearchViewModel in productSearchViewModels)
            {
                BuildProductBadgeValues(productBadgesLookupValues, productSearchViewModel);
                BuildPricePerUnit(unitOfMeasureLookupValues, productSearchViewModel);
            }

            return productSearchViewModels;
        }

        public virtual void BuildProductBadgeValues(ILookup<string, LookupValue> productBadgesLookupValues, ProductSearchViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();
            if (extendedVM.ProductBadges == null)
            {
                return;
            }

            extendedVM.ProductBadgeValues = new Dictionary<string, string>();
            foreach (var extendedVmProductBadge in extendedVM.ProductBadges)
            {
                if (!productBadgesLookupValues.Contains(extendedVmProductBadge)) continue;

                var value = productBadgesLookupValues[extendedVmProductBadge].FirstOrDefault()?.Value;
                if (value != null)
                    extendedVM.ProductBadgeValues.Add(value, extendedVmProductBadge);
            }

            productSearchViewModel.Context["ProductBadgeValues"] = extendedVM.ProductBadgeValues;
        }

        public virtual void BuildPricePerUnit(ILookup<string, LookupValue>  unitOfMeasureLookupValues, ProductSearchViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();

            if (string.IsNullOrEmpty(extendedVM.BaseProductMeasure) ||
                string.IsNullOrEmpty(extendedVM.ProductUnitMeasure))
                return;

            var baseProductMeasureValue = unitOfMeasureLookupValues.Contains(extendedVM.BaseProductMeasure)
                ? unitOfMeasureLookupValues[extendedVM.BaseProductMeasure].FirstOrDefault()?.Value
                : extendedVM.BaseProductMeasure;

            var weightVolumeQuantityMeasureValue =
                unitOfMeasureLookupValues.Contains(extendedVM.ProductUnitMeasure)
                    ? unitOfMeasureLookupValues[extendedVM.ProductUnitMeasure].FirstOrDefault()?.Value
                    : extendedVM.BaseProductMeasure;

            var convertedVolumeMeasurment = (decimal)ConverterProvider.ConvertMeasurements(extendedVM.BaseProductSize,
                baseProductMeasureValue, weightVolumeQuantityMeasureValue);

            extendedVM.ConvertedVolumeMeasurement = convertedVolumeMeasurment;
            productSearchViewModel.Context["ConvertedVolumeMeasurement"] = convertedVolumeMeasurment;
            productSearchViewModel.Context["BaseProductMeasure"] = extendedVM.BaseProductMeasure;
        }

        private ILookup<string, LookupValue> GetLookupValuesByDisplayNameKeys(List<Lookup> lookups, string cultureName, string lookupName)
        {
            return lookups
                ?.FirstOrDefault(item => item.LookupName == lookupName)
                ?.Values
                .ToLookup(_ =>
                {
                    var localizedDisplayName = _.DisplayName.GetLocalizedValue(cultureName);
                    
                    if (!string.IsNullOrEmpty(localizedDisplayName)) return localizedDisplayName;

                    return _.Value; 
                });
        }
    }
}
