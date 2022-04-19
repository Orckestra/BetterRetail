using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Grocery.Context;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Cart.Parameters;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryLineItemViewModelFactory : LineItemViewModelFactory
    {
        public IConverterProvider ConverterProvider { get; }
        public IProductTileConfigurationContext ProductTileConfigurationContext { get; }

        public GroceryLineItemViewModelFactory(IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider,
            IProductUrlProvider productUrlProvider,
            IRewardViewModelFactory rewardViewModelFactory,
            ILineItemValidationProvider lineItemValidationProvider,
            IRecurringOrdersRepository recurringOrderRepository,
            IComposerContext composerContext,
            IRecurringOrderProgramViewModelFactory recurringOrderProgramViewModelFactory,
            IRecurringOrdersSettings recurringOrdersSettings,
            ICurrencyProvider currencyProvider,
            IConverterProvider converterProvider,
            IProductTileConfigurationContext productTileConfigurationContext)
            : base(viewModelMapper,
             localizationProvider,
             productUrlProvider,
             rewardViewModelFactory,
             lineItemValidationProvider,
             recurringOrderRepository,
             composerContext,
             recurringOrderProgramViewModelFactory,
             recurringOrdersSettings,
             currencyProvider)
        {
            ConverterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
            ProductTileConfigurationContext = productTileConfigurationContext ?? throw new ArgumentNullException(nameof(productTileConfigurationContext));
        }

        public override LineItemDetailViewModel GetLineItemDetailViewModel(CreateLineItemDetailViewModelParam param)
        {
            var lineItemhViewModel = base.GetLineItemDetailViewModel(param);
            BuildProductBadgeValues(lineItemhViewModel);
            BuildPromotionalRibbon(lineItemhViewModel);
            BuildPromotionalBanner(lineItemhViewModel);
            BuildMeasure(lineItemhViewModel);

            return lineItemhViewModel;
        }

        public virtual void BuildProductBadgeValues(LineItemDetailViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryLineItemDetailViewModel>();

            var productBadges = extendedVM.ProductBadges?.Split('|');
            if (productBadges == null || !productBadges.Any())
            {
                return;
            }

            var productBadgesLookup = extendedVM.ProductBadgesLookup?.Split(new[] { ", " }, StringSplitOptions.None);
            if (productBadgesLookup == null || !productBadgesLookup.Any())
            {
                return;
            }

            extendedVM.ProductBadgeValues = new Dictionary<string, string>();
            for (var i = 0; i < productBadges.Length; i++)
            {
                var value = productBadges[i];
                var name = productBadgesLookup[i];
                if (!extendedVM.ProductBadgeValues.ContainsKey(value))
                {
                    extendedVM.ProductBadgeValues.Add(value, name);
                }
            }

            productSearchViewModel.Context["ProductBadgeValues"] = extendedVM.ProductBadgeValues;
        }

        public virtual void BuildPromotionalRibbon(LineItemDetailViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryLineItemDetailViewModel>();
        
            var promotionalRibbonSettings = ProductTileConfigurationContext.GetPromotionalRibbonConfigurations().FirstOrDefault(item => item.LookupValue == extendedVM.PromotionalRibbonKey);
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

        public virtual void BuildPromotionalBanner(LineItemDetailViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryLineItemDetailViewModel>();
           
            var promotionalBannerSettings = ProductTileConfigurationContext.GetPromotionalBannerConfigurations().FirstOrDefault(item => item.LookupValue == extendedVM.PromotionalBannerKey);
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

        public virtual void BuildMeasure(LineItemDetailViewModel productSearchViewModel)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryLineItemDetailViewModel>();
            extendedVM.HasUnitValues = extendedVM.ProductUnitQuantity > 0 && extendedVM.ProductUnitSize > 0 && !string.IsNullOrWhiteSpace(extendedVM.ProductUnitMeasure);
            extendedVM.IsSingleUnit = extendedVM.ProductUnitQuantity == 1;
        }
    }
}
