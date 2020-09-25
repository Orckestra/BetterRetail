using System.Collections.Generic;
using Orckestra.Composer;
using Orckestra.Composer.Cart;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Product;

namespace Orckestra.Composer.Grocery.Website
{
    public class ComposerConfig
    {
        public static void RegisterConfigurations()
        {
            //ComposerConfigurations
            ComposerConfiguration.ResxLocalizationRepositoryConfiguration.LocalizationResxDirectory = @"~\UI.Package\LocalizedStrings\";
            ComposerConfiguration.HandlebarsViewEngineConfiguration.ViewLocationFormats = new List<string> {
                "~/UI.Package/Templates/{1}/{0}.hbs",
                "~/UI.Package/Templates/{0}.hbs",
                "~/Views/{1}/{0}.hbs",
                "~/Views/Shared/{0}.hbs"
            };

            //CartConfiguration
            CartConfiguration.ShoppingCartName = "Default";

            //ProductConfiguration
            ComposerConfiguration.AvailableStatusForSell = new List<InventoryStatusEnum>
            {
                InventoryStatusEnum.InStock
            };

            //ProductConfiguration
            SpecificationConfiguration.ExcludedSpecificationPropertyGroups = new List<string>{ "Default" };
            SpecificationConfiguration.ExcludedSpecificationProperty = new List<string> { "ProductDefinition" };
        }
    }
}
