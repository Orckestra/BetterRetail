using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Product;
using Orckestra.Composer.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Factory
{
    public class ProductPromotionsFactory : IProductPromotionsFactory
    {
        public IProductTileConfigurationContext ProductTileConfigurationContext { get; }
        protected ILocalizationProvider LocalizationProvider { get; }

        public ProductPromotionsFactory(
            ILocalizationProvider localizationProvider,
            IProductTileConfigurationContext productTileConfigurationContext)
        {
            ProductTileConfigurationContext = productTileConfigurationContext ?? throw new ArgumentNullException(nameof(productTileConfigurationContext));
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        public virtual Dictionary<string, string> BuildProductBadgeValues(string keys, string lookupDisplayNames)
        {   
            if (string.IsNullOrWhiteSpace(keys)) return null;
            var productBadgesNames = !string.IsNullOrWhiteSpace(lookupDisplayNames) ? lookupDisplayNames.Split(',').ToList() : new List<string>();
            var productBadgeKeys = keys.Split('|').ToList();

           var productBadgeValues = new Dictionary<string, string>();
            for (var i = 0; i < productBadgeKeys.Count; i++)
            {
                if (!productBadgeValues.ContainsValue(productBadgeKeys[i]))
                {
                    var displayName = productBadgesNames.Count > i ? productBadgesNames[i] : productBadgeKeys[i];
                    productBadgeValues.Add(productBadgeKeys[i], displayName);
                }
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

        public virtual string BuildProductVariantColor(string variantValue)
        {
            var settings = ProductTileConfigurationContext.GetVariantColorConfigurations()
                   .FirstOrDefault(item => item.LookupValue.Equals(variantValue, StringComparison.OrdinalIgnoreCase));
            if(settings != null)
            {
                if(!string.IsNullOrEmpty(settings.Color))
                {
                    return settings.Color;
                }

                if (!string.IsNullOrEmpty(settings.Image))
                {
                    return $"url({ProductConfiguration.ColorVarinatImagesRootPath}{settings.Image}) center repeat";
                }
            }

            return null;
        }
    }
}
