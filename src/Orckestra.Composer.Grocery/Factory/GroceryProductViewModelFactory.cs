using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Grocery.Context;
using Orckestra.Composer.Grocery.ViewModels;

namespace Orckestra.Composer.Grocery.Factory
{
    public class GroceryProductViewModelFactory: ProductViewModelFactory
    {
        public IConverterProvider ConverterProvider { get; }
        public IProductTileConfigurationContext ProductTileConfigurationContext { get; }
        
        public GroceryProductViewModelFactory(IViewModelMapper viewModelMapper,
            IProductRepository productRepository, 
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            ILookupService lookupService, 
            IProductUrlProvider productUrlProvider, 
            IScopeViewService scopeViewService, 
            IRecurringOrdersRepository recurringOrdersRepository,
            IRecurringOrderProgramViewModelFactory recurringOrderProgramViewModelFactory,
            IRecurringOrdersSettings recurringOrdersSettings, 
            IProductSpecificationsViewService productSpecificationsViewService,
            IMyAccountUrlProvider myAccountUrlProvider,
            IConverterProvider converterProvider,
            IProductTileConfigurationContext productTileConfigurationContext)
            : base(viewModelMapper, 
                productRepository,
                damProvider,
                localizationProvider, 
                lookupService, 
                productUrlProvider,
                scopeViewService,
                recurringOrdersRepository,
                recurringOrderProgramViewModelFactory,
                recurringOrdersSettings,
                productSpecificationsViewService,
                myAccountUrlProvider)
        {
            ConverterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
            ProductTileConfigurationContext = productTileConfigurationContext ?? throw new ArgumentNullException(nameof(productTileConfigurationContext));
        }

        protected override ProductViewModel CreateViewModel(CreateProductDetailViewModelParam param)
        {
            var productViewModel = base.CreateViewModel(param);
            var extendedVM = productViewModel.AsExtensionModel<IGroceryProductViewModel>();
         
            extendedVM.ProductBadgeValues = BuildProductBadgeValues(param.Product, productViewModel);

            var convertedVolumeMeasurment = BuildConvertedVolumeMeasurement(param.Product);
            if (convertedVolumeMeasurment.HasValue) {
                extendedVM.ConvertedVolumeMeasurement = convertedVolumeMeasurment.Value;
                productViewModel.Context["ConvertedVolumeMeasurement"] = convertedVolumeMeasurment.Value;
                productViewModel.Context["BaseProductMeasure"] = extendedVM.BaseProductMeasure;
            }

            return productViewModel;
        }

        public virtual Dictionary<string, string> BuildProductBadgeValues(Overture.ServiceModel.Products.Product product, BaseProductViewModel productViewModel)
        {
            var productBadges = productViewModel.Bag.TryGetValue("ProductBadges", out var badges) && badges != null ? badges.ToString().Split('|').ToList() : new List<string>();
            var productBadgeKeys = product.PropertyBag.TryGetValue("ProductBadges", out var badgekeys) && badgekeys != null ? badgekeys.ToString().Split('|').ToList() : new List<string>();

            if (!productBadgeKeys.Any())
            {
                return null;
            }

            var productBadgeValues = new Dictionary<string, string>();

            for (var i = 0; i < productBadgeKeys.Count; i++)
            {
                if (!productBadgeValues.ContainsValue(productBadgeKeys[i]))
                    productBadgeValues.Add(productBadgeKeys[i], productBadges[i]);
            }

            return productBadgeValues;
        }

        public virtual decimal? BuildConvertedVolumeMeasurement(Overture.ServiceModel.Products.Product product)
        {
            var baseProductSize = product.PropertyBag.TryGetValue("BaseProductSize", out var sizeValue) ? double.Parse(sizeValue.ToString()) : 0;
            var baseProductMeasure = product.PropertyBag.TryGetValue("BaseProductMeasure", out var measureValue) ? measureValue.ToString() : string.Empty;
            var productUnitMeasure = product.PropertyBag.TryGetValue("ProductUnitMeasure", out var unitMeasureValue) ? unitMeasureValue.ToString() : string.Empty;
            if (string.IsNullOrEmpty(baseProductMeasure) || string.IsNullOrEmpty(productUnitMeasure))
            {
                return null;
            }

            return (decimal)ConverterProvider.ConvertMeasurements(baseProductSize, baseProductMeasure, productUnitMeasure);
        }

        public virtual (string BackgroundColor, string TextColor) BuildPromotionalRibbonStyles(Overture.ServiceModel.Products.Product product)
        {
            var defaultBackgroundColor = ProductTileConfigurationContext.PromotionalRibbonDefaultBackgroundColor;
            var defaultTextColor = ProductTileConfigurationContext.PromotionalRibbonDefaultTextColor;

            var getPromotionalRibbonValue =
                product.PropertyBag.TryGetValue("PromotionalRibbon", out var promotionalRibbonValue);
            if (getPromotionalRibbonValue)
            {
                var promotionalRibbonSettings = ProductTileConfigurationContext.GetPromotionalRibbonConfigurations()
                    .FirstOrDefault(item => item.LookupValue == promotionalRibbonValue.ToString());
                var backgroundColor = promotionalRibbonSettings != null
                    ? promotionalRibbonSettings.BackgroundColor
                    : defaultBackgroundColor;

                var textColor = promotionalRibbonSettings != null
                    ? promotionalRibbonSettings.TextColor
                    : defaultTextColor;
                return (backgroundColor, textColor);
            }

            return (defaultBackgroundColor, defaultTextColor);
        }

    }
}
