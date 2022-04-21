using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Factory
{
    public interface IGroceryProductInformationFactory
    {
        string BuildProductFormat(int productUnitQuantity,
            decimal productUnitSize,
            string productUnitMeasure,
            bool isWeightedProduct,
            CultureInfo cultureInfo);

        Dictionary<string, string> BuildProductBadgeValues(string keys, string lookupDisplayNames);
        (string BackgroundColor, string TextColor) BuildPromotionalRibbonStyles(string ribbobValue);
        (string BackgroundColor, string TextColor) BuildPromotionalBannerStyles(string bannerValue);
    }
}