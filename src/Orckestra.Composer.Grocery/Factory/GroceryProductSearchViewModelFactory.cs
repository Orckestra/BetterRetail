using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.Context;
using Orckestra.Composer.Grocery.Factory;
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

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryProductSearchViewModelFactory : ProductSearchViewModelFactory
    {
        public IConverterProvider ConverterProvider { get; }
        public IGroceryProductInformationFactory GroceryProductInformationFactory { get; }

        public GroceryProductSearchViewModelFactory(
            IViewModelMapper viewModelMapper,
            IProductUrlProvider productUrlProvider,
            IRecurringOrdersSettings recurringOrdersSettings,
            IComposerContext composerContext,
            IProductSettingsViewService productSettings,
            IPriceProvider priceProvider,
            IConverterProvider converterProvider,
            IGroceryProductInformationFactory groceryProductInformationFactory)
            : base(
                viewModelMapper,
                productUrlProvider,
                recurringOrdersSettings,
                composerContext,
                productSettings,
                priceProvider)
        {
            ConverterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
            GroceryProductInformationFactory = groceryProductInformationFactory ?? throw new ArgumentNullException(nameof(groceryProductInformationFactory));
        }

        public override ProductSearchViewModel GetProductSearchViewModel(ProductDocument productDocument, SearchCriteria criteria,
            IDictionary<(string ProductId, string VariantId), ProductMainImage> imgDictionary)
        {
            var productSearchViewModel = base.GetProductSearchViewModel(productDocument, criteria, imgDictionary);
            var vm = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();
            vm.Format = GroceryProductInformationFactory.BuildProductFormat(vm.ProductUnitQuantity,
                (decimal)vm.ProductUnitSize,
                vm.ProductUnitMeasure,
                productSearchViewModel.IsWeightedProduct,
                criteria.CultureInfo);

            BuildPricePerUnit(productDocument, productSearchViewModel);

            if (vm.ProductBadges != null)
            {
                vm.ProductBadgeValues = GroceryProductInformationFactory.BuildProductBadgeValues(base.ExtractLookupId("ProductBadges_Facet", productDocument.PropertyBag), string.Join(",", vm.ProductBadges));
            }

            var ribbonStyles = GroceryProductInformationFactory.BuildPromotionalRibbonStyles(base.ExtractLookupId("PromotionalRibbon_Facet", productDocument.PropertyBag));
            vm.PromotionalRibbonBackgroundColor = ribbonStyles.BackgroundColor;
            vm.PromotionalRibbonTextColor = ribbonStyles.TextColor;

            var bannerStyles = GroceryProductInformationFactory.BuildPromotionalBannerStyles(base.ExtractLookupId("PromotionalBanner_Facet", productDocument.PropertyBag));
            vm.PromotionalBannerBackgroundColor = bannerStyles.BackgroundColor;
            vm.PromotionalBannerTextColor = bannerStyles.TextColor;

            return productSearchViewModel;
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
    }
}
