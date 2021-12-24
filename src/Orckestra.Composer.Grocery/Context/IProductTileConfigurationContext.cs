using System.Collections.Generic;
using Orckestra.Composer.Grocery.DataTypes;

namespace Orckestra.Composer.Grocery.Context
{
    public interface IProductTileConfigurationContext
    {
        string PromotionalRibbonDefaultBackgroundColor { get; }
        string PromotionalRibbonDefaultTextColor { get; }
        List<IPromotionalRibbonConfiguration> GetPromotionalRibbonConfigurations();
    }
}
