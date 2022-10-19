using System.Collections.Generic;
using Orckestra.Composer.CompositeC1.DataTypes;

namespace Orckestra.Composer.CompositeC1.Context
{
    public interface IProductTileConfigurationContext
    {
        string PromotionalRibbonDefaultBackgroundColor { get; }
        string PromotionalRibbonDefaultTextColor { get; }
        string PromotionalBannerDefaultBackgroundColor { get; }
        string PromotionalBannerDefaultTextColor { get; }
        List<IPromotionalRibbonConfiguration> GetPromotionalRibbonConfigurations();
        List<IPromotionalBannerConfiguration> GetPromotionalBannerConfigurations();
    }
}
