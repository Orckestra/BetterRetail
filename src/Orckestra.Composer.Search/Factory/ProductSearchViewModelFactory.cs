using Orckestra.Composer.Configuration;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Products.Inventory;
using Orckestra.Overture.ServiceModel.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Orckestra.Composer.Search.Factory
{
    public class ProductSearchViewModelFactory: IProductSearchViewModelFactory
    {
        private const string VariantPropertyBagKey = "VariantId";

        protected IViewModelMapper ViewModelMapper { get; }
        protected IProductUrlProvider ProductUrlProvider { get; }
        protected IRecurringOrdersSettings RecurringOrdersSettings { get; private set; }

        protected ProductSearchViewModelFactory(
            IViewModelMapper viewModelMapper, 
            IProductUrlProvider productUrlProvider, 
            IRecurringOrdersSettings recurringOrdersSettings)
        {
            ViewModelMapper = viewModelMapper ?? throw new ArgumentNullException(nameof(viewModelMapper));
            ProductUrlProvider = productUrlProvider ?? throw new ArgumentNullException(nameof(productUrlProvider));
            RecurringOrdersSettings = recurringOrdersSettings ?? throw new ArgumentNullException(nameof(recurringOrdersSettings));
        }

        public virtual ProductSearchViewModel GetProductSearchViewModel(ProductDocument productDocument, SearchCriteria criteria, IDictionary<(string, string), ProductMainImage> imgDictionary)
        {
            var cultureInfo = criteria.CultureInfo;

            string variantId = null;
            if (productDocument.PropertyBag.ContainsKey(VariantPropertyBagKey))
            {
                variantId = productDocument.PropertyBag[VariantPropertyBagKey] as string;
            }

            var productSearchVm = ViewModelMapper.MapTo<ProductSearchViewModel>(productDocument, cultureInfo);
            productSearchVm.ProductId = productDocument.ProductId;
            productSearchVm.BrandId = ExtractLookupId("Brand_Facet", productDocument.PropertyBag);
            MapProductSearchViewModelInfos(productSearchVm, productDocument, cultureInfo);
            MapProductSearchViewModelUrl(productSearchVm, variantId, cultureInfo, criteria.BaseUrl);
            MapProductSearchViewModelImage(productSearchVm, imgDictionary);

            productSearchVm.IsRecurringOrderEligible = RecurringOrdersSettings.Enabled && productDocument.PropertyBag.IsRecurringOrderEligible();
            productSearchVm.Context["IsRecurringOrderEligible "] = productSearchVm.IsRecurringOrderEligible;

            return productSearchVm;
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
          IDictionary<(string, string), ProductMainImage> imgDictionary)
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

        public virtual void MapProductSearchViewModelAvailableForSell(ProductSearchViewModel productSearchViewModel, ProductDocument productDocument, bool IsInventoryEnabled)
        {
            if (!IsInventoryEnabled) { 
                productSearchViewModel.IsAvailableToSell = true;
                return;
            }

            if (productSearchViewModel.HasVariants) {
                productSearchViewModel.IsAvailableToSell = true;
                return;
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

        public virtual void MapProductSearchViewModelPricing(ProductSearchViewModel productSearchVm, ProductPriceSearchViewModel pricing)
        {
            productSearchVm.DisplayListPrice = pricing.DisplayPrice;
            productSearchVm.DisplaySpecialPrice = pricing.DisplaySpecialPrice;
            productSearchVm.HasPriceRange = pricing.HasPriceRange;
            productSearchVm.ListPrice = pricing.ListPrice;
            productSearchVm.Price = pricing.Price;
            productSearchVm.IsOnSale = pricing.IsOnSale;
            productSearchVm.PriceListId = pricing.PriceListId;
        }
    }
}
