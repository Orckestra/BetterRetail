using Orckestra.Composer.Grocery.Context;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Orckestra.Composer.Grocery.Factory
{
    public class GroceryProductInformationFactory : IGroceryProductInformationFactory
    {
        public IProductTileConfigurationContext ProductTileConfigurationContext { get; }
        protected ILocalizationProvider LocalizationProvider { get; }

        public GroceryProductInformationFactory(
            ILocalizationProvider localizationProvider,
            IProductTileConfigurationContext productTileConfigurationContext)
        {
            ProductTileConfigurationContext = productTileConfigurationContext ?? throw new ArgumentNullException(nameof(productTileConfigurationContext));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }
      public virtual string BuildProductFormat(int productUnitQuantity,
            decimal productUnitSize,
            string productUnitMeasure,
            bool isWeightedProduct,
            CultureInfo cultureInfo)
        {
            var hasUnitValues = productUnitQuantity > 0 && productUnitSize > 0 && !string.IsNullOrWhiteSpace(productUnitMeasure);

            if (hasUnitValues)
            {
                var isSingleUnit = productUnitQuantity == 1;
                var unitQuantity = (isSingleUnit) ? "" : $"{productUnitQuantity} x";
                var unitQuantitySuffix = (isSingleUnit && isWeightedProduct) ? LocalizationProvider.GetLocalizedString(new Composer.Providers.Localization.GetLocalizedParam() { Category = "General", Key = "L_EachAbbrev", CultureInfo = cultureInfo }) : "";
                var unitSize = (isWeightedProduct) ? $"{LocalizationProvider.GetLocalizedString(new Composer.Providers.Localization.GetLocalizedParam() { Category = "ProductPage", Key = "L_Approx", CultureInfo = cultureInfo })} {productUnitSize}{productUnitMeasure}" : $"{productUnitSize}{productUnitMeasure}";
                return $"{unitQuantity} {unitSize} {unitQuantitySuffix}".TrimEnd().TrimStart();
            }

            return null;
        }

        public virtual Dictionary<string, string> BuildProductBadgeValues(string keys, string lookupDisplayNames)
        {
            var productBadgesNames = lookupDisplayNames != null ? lookupDisplayNames.Split(',').ToList() : new List<string>();
            var productBadgeKeys = keys != null ? keys.Split('|').ToList() : new List<string>();

            if (!productBadgeKeys.Any())
            {
                return null;
            }

            var productBadgeValues = new Dictionary<string, string>();

            for (var i = 0; i < productBadgeKeys.Count; i++)
            {
                if (!productBadgeValues.ContainsValue(productBadgeKeys[i]))
                    productBadgeValues.Add(productBadgeKeys[i], productBadgesNames[i]);
            }

            return productBadgeValues;
        }

        public virtual (string BackgroundColor, string TextColor) BuildPromotionalRibbonStyles(string ribbobValue)
        {
            var defaultBackgroundColor = ProductTileConfigurationContext.PromotionalRibbonDefaultBackgroundColor;
            var defaultTextColor = ProductTileConfigurationContext.PromotionalRibbonDefaultTextColor;

            if (!string.IsNullOrWhiteSpace(ribbobValue))
            {
                var promotionalRibbonSettings = ProductTileConfigurationContext.GetPromotionalRibbonConfigurations()
                    .FirstOrDefault(item => item.LookupValue == ribbobValue);
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

        public virtual (string BackgroundColor, string TextColor) BuildPromotionalBannerStyles(string bannerValue)
        {
            var defaultBackgroundColor = ProductTileConfigurationContext.PromotionalBannerDefaultBackgroundColor;
            var defaultTextColor = ProductTileConfigurationContext.PromotionalBannerDefaultTextColor;

            if (!string.IsNullOrWhiteSpace(bannerValue))
            {
                var promotionalBannerSettings = ProductTileConfigurationContext.GetPromotionalBannerConfigurations()
                    .FirstOrDefault(item => item.LookupValue == bannerValue.ToString());
                var backgroundColor = promotionalBannerSettings != null && promotionalBannerSettings.BackgroundColor != "bg-none"
                    ? promotionalBannerSettings.BackgroundColor
                    : defaultBackgroundColor;

                var textColor = promotionalBannerSettings != null && promotionalBannerSettings?.TextColor != "text-none"
                    ? promotionalBannerSettings.TextColor
                    : defaultTextColor;
                return (backgroundColor, textColor);
            }

            return (defaultBackgroundColor, defaultTextColor);
        }
    }
}
