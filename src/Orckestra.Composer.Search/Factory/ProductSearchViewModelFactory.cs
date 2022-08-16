using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Factory
{
    public class ProductSearchViewModelFactory: IProductSearchViewModelFactory
    {
        private const string VariantPropertyBagKey = "VariantId";

        protected IViewModelMapper ViewModelMapper { get; }
        protected IProductUrlProvider ProductUrlProvider { get; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }
        protected IProductSettingsViewService ProductSettings { get; }
        protected IComposerContext ComposerContext { get; }
        protected IPriceProvider PriceProvider { get; }
        protected IProductInformationFactory ProductInformationFactory { get; }

        public ProductSearchViewModelFactory(
            IViewModelMapper viewModelMapper, 
            IProductUrlProvider productUrlProvider, 
            IRecurringOrdersSettings recurringOrdersSettings,
            IComposerContext composerContext,
            IProductSettingsViewService productSettings,
            IPriceProvider priceProvider,
            IProductInformationFactory productInformationFactory)
        {
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            ProductSettings = productSettings ?? throw new ArgumentNullException(nameof(productSettings));
            PriceProvider = priceProvider ?? throw new ArgumentNullException(nameof(priceProvider));
            ProductInformationFactory = productInformationFactory ?? throw new ArgumentNullException(nameof(productInformationFactory));

        }

        public virtual ProductSearchViewModel GetProductSearchViewModel(ProductDocument productDocument, SearchCriteria criteria, IDictionary<(string ProductId, string VariantId), ProductMainImage> imgDictionary)
        {
            var cultureInfo = criteria.CultureInfo;

            string variantId = null;
            if (productDocument.PropertyBag.ContainsKey(VariantPropertyBagKey))
            {
                variantId = productDocument.PropertyBag[VariantPropertyBagKey] as string;
            }

            var vm = ViewModelMapper.MapTo<ProductSearchViewModel>(productDocument, cultureInfo);
            vm.ProductId = productDocument.ProductId;
            MapProductSearchViewModelInfos(vm, productDocument, cultureInfo);
            MapProductSearchViewModelUrl(vm, variantId, cultureInfo, criteria.BaseUrl);
            MapProductSearchViewModelImage(vm, imgDictionary);
            MapProductSearchPromotions(vm, productDocument);

            vm.IsRecurringOrderEligible = RecurringOrdersSettings.Enabled && productDocument.PropertyBag.IsRecurringOrderEligible();
            vm.Context["IsRecurringOrderEligible "] = vm.IsRecurringOrderEligible;

            return vm;
        }

        protected virtual void MapProductSearchPromotions(ProductSearchViewModel vm, ProductDocument productDocument)
        {
            if (vm.ProductBadges != null)
            {
                vm.ProductBadgeValues = ProductInformationFactory.BuildProductBadgeValues(ExtractLookupId("ProductBadges_Facet", productDocument.PropertyBag), string.Join(",", vm.ProductBadges));
            }

            var ribbonStyles = ProductInformationFactory.BuildPromotionalRibbonStyles(ExtractLookupId("PromotionalRibbon_Facet", productDocument.PropertyBag));
            vm.PromotionalRibbonBackgroundColor = ribbonStyles.BackgroundColor;
            vm.PromotionalRibbonTextColor = ribbonStyles.TextColor;

            var bannerStyles = ProductInformationFactory.BuildPromotionalBannerStyles(ExtractLookupId("PromotionalBanner_Facet", productDocument.PropertyBag));
            vm.PromotionalBannerBackgroundColor = bannerStyles.BackgroundColor;
            vm.PromotionalBannerTextColor = bannerStyles.TextColor;
        }

        protected virtual string ExtractLookupId(string fieldName, PropertyBag propertyBag)
        {
            if (propertyBag == null) { return null; }
            var fieldValue = propertyBag.TryGetValue(fieldName, out object value)
                ? (value as string ?? ((string[])value).First())
                : null;
            if (string.IsNullOrWhiteSpace(fieldValue)) { return null; }

            var extractedValues = fieldValue.Split(new[] { "::" }, StringSplitOptions.None);
            return extractedValues.Length < 3
                ? null
                : extractedValues[2];
        }

        protected virtual void MapProductSearchViewModelInfos(
           ProductSearchViewModel productSearchVm,
           ProductDocument productDocument,
           CultureInfo cultureInfo)
        {
            productSearchVm.DisplayName = TrimProductDisplayName(productSearchVm.FullDisplayName);
            productSearchVm.Description = null; // We don't need Description in Search Results, setting to null, to reduce HTML size


            //TODO use ProductDocument property when overture will have add it.
            if (productSearchVm.Bag.ContainsKey("DefinitionName"))
            {
                productSearchVm.DefinitionName = productSearchVm.Bag["DefinitionName"].ToString();
            }

            productSearchVm.HasVariants = HasVariants(productDocument);
        }

        private static string TrimProductDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) { return string.Empty; }

            var trimmedDisplayName = displayName.Substring(0, Math.Min(displayName.Length, DisplayConfiguration.ProductNameMaxLength));

            return trimmedDisplayName;
        }

        protected static bool HasVariants(ProductDocument resultItem)
        {
            if (resultItem?.PropertyBag == null || !resultItem.PropertyBag.TryGetValue("GroupCount", out object variantCountObject) || variantCountObject == null)
            {
                return false;
            }

            var variantCountString = variantCountObject.ToString();

            int.TryParse(variantCountString, out int result);

            return result > 0;
        }

        protected virtual void MapProductSearchViewModelUrl(
            ProductSearchViewModel productSearchVm,
            string productVariantId,
            CultureInfo cultureInfo, string baseUrl)
        {
            productSearchVm.Url = ProductUrlProvider.GetProductUrl(new GetProductUrlParam
            {
                CultureInfo = cultureInfo,
                ProductId = productSearchVm.ProductId,
                ProductName = productSearchVm.FullDisplayName,
                VariantId = productVariantId,
                BaseUrl = baseUrl,
                SKU = productSearchVm.Sku
            });
        }

        protected virtual void MapProductSearchViewModelImage(
          ProductSearchViewModel productSearchVm,
          IDictionary<(string ProductId, string VariantId), ProductMainImage> imgDictionary)
        {
            string productVariantId = null;
            if (!string.IsNullOrWhiteSpace(productSearchVm.VariantId))
            {
                productVariantId = productSearchVm.VariantId;
            }

            var imageKey = (productSearchVm.ProductId, productVariantId);
            var imageExists = imgDictionary.TryGetValue(imageKey, out ProductMainImage mainImage);

            if (imageExists)
            {
                productSearchVm.ImageUrl = mainImage.ImageUrl;
                productSearchVm.FallbackImageUrl = mainImage.FallbackImageUrl;
            }
        }

        protected virtual void MapProductSearchViewModelAvailableForSell(ProductSearchViewModel productSearchViewModel, ProductDocument productDocument, ProductSettingsViewModel productSettings)
        {
            if (!productSettings.IsInventoryEnabled) { 
                productSearchViewModel.IsAvailableToSell = true;
                return;
            }

            if (productSearchViewModel.HasVariants) {
                productSearchViewModel.IsAvailableToSell = true;
                return;
            }

            if(productDocument.InventoryLocationStatuses == null)
            {
                throw new InvalidOperationException("Product document doesn't contain inventory information");
            }

            var availableStatusesForSell = ComposerConfiguration.AvailableStatusForSell;

            productSearchViewModel.IsAvailableToSell = productDocument.InventoryLocationStatuses.SelectMany(_ => _.Statuses)
                .Select(status => GetInventoryItemStatus(status.Status))
                .Any(availableStatusesForSell.Contains);
        }


        protected static InventoryStatusEnum GetInventoryItemStatus(InventoryStatus inventoryStatus)
        {
            switch (inventoryStatus)
            {
                case InventoryStatus.InStock:
                    return InventoryStatusEnum.InStock;
                case InventoryStatus.OutOfStock:
                    return InventoryStatusEnum.OutOfStock;
                case InventoryStatus.PreOrder:
                    return InventoryStatusEnum.PreOrder;
                case InventoryStatus.BackOrder:
                    return InventoryStatusEnum.BackOrder;
                default:
                    return InventoryStatusEnum.Unspecified;
            }
        }

        protected virtual void MapProductSearchViewModelPricing(ProductSearchViewModel productSearchVm, ProductPriceSearchViewModel pricing)
        {
            productSearchVm.DisplayListPrice = pricing.DisplayPrice;
            productSearchVm.DisplaySpecialPrice = pricing.DisplaySpecialPrice;
            productSearchVm.HasPriceRange = pricing.HasPriceRange;
            productSearchVm.ListPrice = pricing.ListPrice;
            productSearchVm.Price = pricing.Price;
            productSearchVm.IsOnSale = pricing.IsOnSale;
            productSearchVm.PriceListId = pricing.PriceListId;
        }

        /// <remarks>
        /// NOTE: when fetching data for products from OCC APIs, make sure to query data in batches for optimal performance
        /// https://docs.orckestra.com/developer-documentation/platform-performance/batch-api-requests
        /// </remarks>
        public virtual async Task<IList<ProductSearchViewModel>> EnrichAppendProductSearchViewModels(IList<(ProductSearchViewModel, ProductDocument)> productSearchResultList, SearchCriteria criteria)
        {
            var productSettings = await ProductSettings.GetProductSettings(criteria.Scope, criteria.CultureInfo).ConfigureAwait(false);

            foreach (var (productSearchVm, productDocument) in productSearchResultList)
            {
                MapProductSearchViewModelAvailableForSell(productSearchVm, productDocument, productSettings);
                var pricing = await PriceProvider.GetPriceAsync(productSearchVm.HasVariants, productDocument).ConfigureAwait(false);
                MapProductSearchViewModelPricing(productSearchVm, pricing);
            }

            return productSearchResultList.Select((resultItem) => resultItem.Item1).ToList();
        }
    }
}
