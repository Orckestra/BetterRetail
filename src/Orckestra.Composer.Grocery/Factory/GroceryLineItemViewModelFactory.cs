using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.LineItemValidation;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Grocery.Context;
using Orckestra.Composer.Grocery.Factory;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using System;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryLineItemViewModelFactory : LineItemViewModelFactory
    {
        public IConverterProvider ConverterProvider { get; }
        public IProductTileConfigurationContext ProductTileConfigurationContext { get; }

        public IGroceryProductInformationFactory GroceryProductInformationFactory { get; }

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
            IGroceryProductInformationFactory groceryProductInformationFactory)
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
           GroceryProductInformationFactory = groceryProductInformationFactory ?? throw new ArgumentNullException(nameof(groceryProductInformationFactory));
        }

        public override LineItemDetailViewModel GetLineItemDetailViewModel(CreateLineItemDetailViewModelParam param)
        {
            var lineItemhViewModel = base.GetLineItemDetailViewModel(param);
            var extendedVM = lineItemhViewModel.AsExtensionModel<IGroceryLineItemDetailViewModel>();
            extendedVM.ProductBadgeValues = GroceryProductInformationFactory.BuildProductBadgeValues(extendedVM.ProductBadges, extendedVM.ProductBadgesLookup);
            
            var ribbonStyles = GroceryProductInformationFactory.BuildPromotionalRibbonStyles(extendedVM.PromotionalRibbonKey);
            extendedVM.PromotionalRibbonBackgroundColor = ribbonStyles.BackgroundColor;
            extendedVM.PromotionalRibbonTextColor = ribbonStyles.TextColor;

            var bannerStyles = GroceryProductInformationFactory.BuildPromotionalBannerStyles(extendedVM.PromotionalBannerKey);
            extendedVM.PromotionalBannerBackgroundColor = bannerStyles.BackgroundColor;
            extendedVM.PromotionalBannerTextColor = bannerStyles.TextColor;

            extendedVM.Format = GroceryProductInformationFactory.BuildProductFormat(extendedVM.ProductUnitQuantity, extendedVM.ProductUnitSize, extendedVM.ProductUnitMeasure, extendedVM.IsWeightedProduct, param.CultureInfo);

            return lineItemhViewModel;
        }
    }
}
