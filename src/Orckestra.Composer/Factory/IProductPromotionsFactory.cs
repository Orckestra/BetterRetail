using System.Collections.Generic;

namespace Orckestra.Composer.Factory
{
    public interface IProductPromotionsFactory
    {
        Dictionary<string, string> BuildProductBadgeValues(string keys, string lookupDisplayNames);
        (string BackgroundColor, string TextColor) BuildPromotionalRibbonStyles(string ribbobValue);
        (string BackgroundColor, string TextColor) BuildPromotionalBannerStyles(string bannerValue);
    }
}