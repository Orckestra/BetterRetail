using System.Collections.Generic;
using Orckestra.Composer.Cart;
using Orckestra.Composer.CompositeC1.Cache;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Product;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.CompositeC1.Mvc
{
    public class ComposerConfig
    {
        public static void RegisterConfigurations()
        {
            //ComposerConfigurations
            ComposerConfiguration.CountryCode = "CA";
            ComposerConfiguration.ResxLocalizationRepositoryConfiguration.LocalizationResxDirectory = @"~\UI.Package\LocalizedStrings\";
            ComposerConfiguration.HandlebarsViewEngineConfiguration.ViewLocationFormats = new List<string> {
                "~/UI.Package/Templates/{1}/{0}.hbs",
                "~/UI.Package/Templates/{0}.hbs",
                "~/Views/{1}/{0}.hbs",
                "~/Views/Shared/{0}.hbs"
            };

            //SearchConfigurations
            SearchConfiguration.FacetSettings = new List<FacetSetting>
            {
                new FacetSetting("CategoryLevel1_Facet")
                {
                    FacetType              = FacetType.SingleSelect,
                    SortWeight             = -1.0,
                    MaxCollapsedValueCount = 5
                },
                new FacetSetting("CategoryLevel2_Facet")
                {
                    FacetType              = FacetType.SingleSelect,
                    SortWeight             = -1.0,
                    MaxCollapsedValueCount = 5,
                    DependsOn = new []
                    {
                        "CategoryLevel1_Facet"
                    }
                },
                new FacetSetting("CategoryLevel3_Facet")
                {
                    FacetType = FacetType.SingleSelect,
                    SortWeight = -1.1,
                    MaxCollapsedValueCount = 5,
                    DependsOn = new[]
                    {
                        "CategoryLevel2_Facet"
                    }
                },
                new FacetSetting("Brand")
                {
                    FacetType              = FacetType.MultiSelect,
                    SortWeight             = 0.0,
                    MaxCollapsedValueCount = 5,
                    MaxExpendedValueCount  = 20
                },
                new FacetSetting("SeasonWear")
                {
                    FacetType              = FacetType.MultiSelect,
                    SortWeight             = 1.0,
                    MaxCollapsedValueCount = 5
                },
                new FacetSetting("ShirtType")
                {
                    FacetType              = FacetType.MultiSelect,
                    SortWeight             = 1.5,
                    MaxCollapsedValueCount = 5
                },
                new FacetSetting("ShoeType")
                {
                    FacetType              = FacetType.SingleSelect,
                    SortWeight             = 1.5,
                    MaxCollapsedValueCount = 5
                },
                new FacetSetting("HeelsHeight")
                {
                    FacetType              = FacetType.SingleSelect,
                    SortWeight             = 1.5,
                    MaxCollapsedValueCount = 5
                },
                new FacetSetting("CurrentPrice")
                {
                    FacetType              = FacetType.Range,
                    SortWeight             = 2.0,
                    StartValue = "0",
                    EndValue = "500",
                    GapSize = "1",
                    FacetValueType = typeof(decimal)
                },
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
