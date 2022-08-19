using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.Factory;
using Orckestra.Composer.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var productBadgesNames = lookupDisplayNames?.Split(',').ToList() ?? new List<string>();
            var productBadgeKeys = keys.Split('|').ToList();

           var productBadgeValues = new Dictionary<string, string>();

            foreach (var tuple in productBadgeKeys.Zip(productBadgesNames, (x, y) => (Key: x, Value: y)))
            {
                productBadgeValues[tuple.Key] = tuple.Value;
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
