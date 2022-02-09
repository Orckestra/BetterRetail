using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using Composite.Data;
using Orckestra.Composer.Grocery.Context;
using Orckestra.Composer.Grocery.Settings;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryProductSearchViewModelFactory : ProductSearchViewModelFactory
    {
        public IConverterProvider ConverterProvider { get; }
        public IProductTileConfigurationContext ProductTileConfigurationContext { get; }

        public GroceryProductSearchViewModelFactory(
            IViewModelMapper viewModelMapper,
            IProductUrlProvider productUrlProvider,
            IRecurringOrdersSettings recurringOrdersSettings,
            IComposerContext composerContext,
            IProductSettingsViewService productSettings,
            IPriceProvider priceProvider,
            IConverterProvider converterProvider,
            IProductTileConfigurationContext productTileConfigurationContext)
            : base(
                viewModelMapper,
                productUrlProvider,
                recurringOrdersSettings,
                composerContext,
                productSettings,
                priceProvider)
        {
            ConverterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
            ProductTileConfigurationContext = productTileConfigurationContext ?? throw new ArgumentNullException(nameof(productTileConfigurationContext));
        }

        public override ProductSearchViewModel GetProductSearchViewModel(ProductDocument productDocument, SearchCriteria criteria,
            IDictionary<(string ProductId, string VariantId), ProductMainImage> imgDictionary)
        {
            var productSearchViewModel = base.GetProductSearchViewModel(productDocument, criteria, imgDictionary);
            BuildProductBadgeValues(productDocument, productSearchViewModel);
            BuildPricePerUnit(productDocument, productSearchViewModel);
            BuildPromotionalRibbon(productDocument, productSearchViewModel);
            BuildPromotionalBanner(productDocument, productSearchViewModel);
            return productSearchViewModel;
        }

        public virtual void BuildProductBadgeValues(ProductDocument productDocument, ProductSearchViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();
            if (extendedVM.ProductBadges == null || !extendedVM.ProductBadges.Any())
            {
                return;
            }

            var productBadgesValuesFromFacet = base.ExtractLookupId("ProductBadges_Facet", productDocument.PropertyBag)
                ?.Split('|');

            if (productBadgesValuesFromFacet == null || !productBadgesValuesFromFacet.Any())
            {
                return;
            }

            extendedVM.ProductBadgeValues = new Dictionary<string, string>();
            for (var i = 0; i < extendedVM.ProductBadges.Length; i++)
            {
                var extendedVmProductBadge = extendedVM.ProductBadges[i];
                var value = productBadgesValuesFromFacet[i];
                if (!extendedVM.ProductBadgeValues.ContainsKey(value))
                {
                    extendedVM.ProductBadgeValues.Add(value, extendedVmProductBadge);
                }
            }

            productSearchViewModel.Context["ProductBadgeValues"] = extendedVM.ProductBadgeValues;
        }

        public virtual void BuildPricePerUnit(ProductDocument productDocument, ProductSearchViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();

            if (string.IsNullOrEmpty(extendedVM.BaseProductMeasure) ||
                string.IsNullOrEmpty(extendedVM.ProductUnitMeasure))
                return;

            var baseProductMeasureValue = base.ExtractLookupId("BaseProductMeasure_Facet", productDocument.PropertyBag);
            var weightVolumeQuantityMeasureValue = base.ExtractLookupId("ProductUnitMeasure_Facet", productDocument.PropertyBag);

            var convertedVolumeMeasurment = (decimal)ConverterProvider.ConvertMeasurements(extendedVM.BaseProductSize,
                baseProductMeasureValue, weightVolumeQuantityMeasureValue);

            extendedVM.ConvertedVolumeMeasurement = convertedVolumeMeasurment;
            productSearchViewModel.Context["ConvertedVolumeMeasurement"] = convertedVolumeMeasurment;
            productSearchViewModel.Context["BaseProductMeasure"] = extendedVM.BaseProductMeasure;
        }

        public virtual void BuildPromotionalRibbon(ProductDocument productDocument, ProductSearchViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();
            var promotionalRibbonPropertyValue = base.ExtractLookupId("PromotionalRibbon_Facet", productDocument.PropertyBag);

            var promotionalRibbonSettings = ProductTileConfigurationContext.GetPromotionalRibbonConfigurations().FirstOrDefault(item => item.LookupValue == promotionalRibbonPropertyValue);
            extendedVM.PromotionalRibbonBackgroundColor = promotionalRibbonSettings?.BackgroundColor != null && promotionalRibbonSettings.BackgroundColor != "bg-none"
                ? promotionalRibbonSettings.BackgroundColor
                : ProductTileConfigurationContext.PromotionalRibbonDefaultBackgroundColor;
            extendedVM.PromotionalRibbonTextColor = promotionalRibbonSettings?.TextColor != null && promotionalRibbonSettings?.TextColor != "text-none"
                ? promotionalRibbonSettings.TextColor
                : ProductTileConfigurationContext.PromotionalRibbonDefaultTextColor;

            productSearchViewModel.Context["PromotionalRibbon"] = extendedVM.PromotionalRibbon;
            productSearchViewModel.Context["PromotionalRibbonDefaultBackgroundColor"] = extendedVM.PromotionalRibbonBackgroundColor;
            productSearchViewModel.Context["PromotionalRibbonDefaultTextColor"] = extendedVM.PromotionalRibbonTextColor;
        }

        public virtual void BuildPromotionalBanner(ProductDocument productDocument, ProductSearchViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();
            var promotionalBannerPropertyValue = base.ExtractLookupId("PromotionalBanner_Facet", productDocument.PropertyBag);

            var promotionalBannerSettings = ProductTileConfigurationContext.GetPromotionalBannerConfigurations().FirstOrDefault(item => item.LookupValue == promotionalBannerPropertyValue);
            extendedVM.PromotionalBannerBackgroundColor = promotionalBannerSettings?.BackgroundColor != null && promotionalBannerSettings.BackgroundColor != "bg-none"
                ? promotionalBannerSettings.BackgroundColor
                : ProductTileConfigurationContext.PromotionalBannerDefaultBackgroundColor;
            extendedVM.PromotionalBannerTextColor = promotionalBannerSettings?.TextColor != null && promotionalBannerSettings?.TextColor != "text-none"
                ? promotionalBannerSettings.TextColor
                : ProductTileConfigurationContext.PromotionalBannerDefaultTextColor;

            productSearchViewModel.Context["PromotionalBanner"] = extendedVM.PromotionalBanner;
            productSearchViewModel.Context["PromotionalBannerDefaultBackgroundColor"] = extendedVM.PromotionalBannerBackgroundColor;
            productSearchViewModel.Context["PromotionalBannerDefaultTextColor"] = extendedVM.PromotionalBannerTextColor;
        }
    }
}
