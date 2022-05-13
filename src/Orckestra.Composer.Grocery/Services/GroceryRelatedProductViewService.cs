using Orckestra.Composer.Configuration;
using Orckestra.Composer.Grocery.Factory;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Product.Factory;
using Orckestra.Composer.Product.Repositories;
using Orckestra.Composer.Product.Services;
using Orckestra.Composer.Product.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryRelatedProductViewService : RelatedProductViewService
    {
        public GroceryProductViewModelFactory GroceryProductViewModelFactory { get; }
        public IConverterProvider ConverterProvider { get; }
        public IGroceryProductInformationFactory GroceryProductInformationFactory { get; }

        public GroceryRelatedProductViewService(
            IProductRepository productRepository,
            IRelationshipRepository relationshipRepository,
            IDamProvider damProvider,
            IProductUrlProvider productUrlProvider,
            IViewModelMapper viewModelMapper,
            ILocalizationProvider localizationProvider,
            IInventoryLocationProvider inventoryLocationProvider,
            IRecurringOrdersSettings recurringOrdersSettings,
            IFulfillmentContext fulfillmentContext,
            IProductViewModelFactory groceryProductViewModelFactory,
            IConverterProvider converterProvider,
            IGroceryProductInformationFactory groceryProductInformationFactory)
            : base(productRepository, relationshipRepository, damProvider, productUrlProvider, viewModelMapper, localizationProvider, inventoryLocationProvider, recurringOrdersSettings, fulfillmentContext)
        {
            GroceryProductViewModelFactory = groceryProductViewModelFactory as GroceryProductViewModelFactory ?? throw new ArgumentNullException(nameof(groceryProductViewModelFactory));
            ConverterProvider = converterProvider ?? throw new ArgumentNullException(nameof(converterProvider));
            GroceryProductInformationFactory = groceryProductInformationFactory ?? throw new ArgumentNullException(nameof(groceryProductInformationFactory));
        }

        protected override RelatedProductViewModel CreateRelatedProductsViewModel(
            Uri baseUrl,
            CultureInfo cultureInfo,
            ProductWithVariant productVariant,
            List<ProductPrice> prices,
            IEnumerable<ProductMainImage> images,
            string currencyIso)
        {
            var relatedProductViewModel = base.CreateRelatedProductsViewModel(baseUrl, cultureInfo, productVariant, prices, images, currencyIso);

            var extendedVM = relatedProductViewModel.AsExtensionModel<IGroceryRelatedProductViewModel>();
            extendedVM.ProductBadgeValues = GroceryProductInformationFactory.BuildProductBadgeValues(extendedVM.ProductBadgesKeys, extendedVM.ProductBadgesLookup);
            extendedVM.Format = GroceryProductInformationFactory.BuildProductFormat(extendedVM.ProductUnitQuantity, extendedVM.ProductUnitSize, extendedVM.ProductUnitMeasure, extendedVM.IsWeightedProduct, cultureInfo);
            var convertedVolumeMeasurment = GroceryProductViewModelFactory.BuildConvertedVolumeMeasurement(productVariant.Product);
            if (convertedVolumeMeasurment.HasValue)
            {
                extendedVM.ConvertedVolumeMeasurement = convertedVolumeMeasurment.Value;
            }

            var ribbonStyles = GroceryProductInformationFactory.BuildPromotionalRibbonStyles(extendedVM.PromotionalRibbonValue);
            extendedVM.PromotionalRibbonBackgroundColor = ribbonStyles.BackgroundColor;
            extendedVM.PromotionalRibbonTextColor = ribbonStyles.TextColor;

            var bannerStyles = GroceryProductInformationFactory.BuildPromotionalBannerStyles(extendedVM.PromotionalBannerValue);
            extendedVM.PromotionalBannerBackgroundColor = bannerStyles.BackgroundColor;
            extendedVM.PromotionalBannerTextColor = bannerStyles.TextColor;

            return relatedProductViewModel;
        }
    }
}