using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using Orckestra.Composer.Search;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.CompositeC1.Mappers
{
    public static class FacetsMapper
    {
        public static FacetSetting ConvertToFacetSetting(IFacet facet, List<IFacet> dependsOn, List<IPromotedFacetValueSetting> promotedFacetValueSettings)
        {
            var facetSetting = new FacetSetting(facet.FieldName)
            {
                FacetType = GetFacetType(facet),
                FacetValueType = GetFacetValueType(facet),
                SortWeight = (double)facet.SortWeight,
                MaxCollapsedValueCount = facet.MaxCollapsedValueCount,
                MaxExpendedValueCount = facet.MaxExpendedValueCount.GetValueOrDefault(int.MaxValue),
                DependsOn = dependsOn.Select(f => f.FieldName).ToList(),
                StartValue = facet.StartValue,
                EndValue = facet.EndValue,
                GapSize = facet.GapSize,
                IsDisplayed = facet.IsDisplayed,
                PromotedValues = promotedFacetValueSettings.Select(ConvertToPromotedFacetValueSetting).ToList(),
            };

            return facetSetting;
        }

        private static FacetType GetFacetType(IFacet facet)
        {
            switch (facet.FacetType)
            {
                case FacetTypeConstants.SingleSelect:
                    return FacetType.SingleSelect;
                case FacetTypeConstants.MultiSelect:
                    return FacetType.MultiSelect;
                case FacetTypeConstants.Range:
                    return FacetType.Range;
                default:
                    throw new ArgumentOutOfRangeException(nameof(IFacet.FacetType));
            }
        }

        private static Type GetFacetValueType(IFacet facet)
        {
            return facet.FacetType == FacetTypeConstants.Range
                ? typeof(decimal)
                : typeof(string);
        }

        private static class FacetTypeConstants
        {
            public const string SingleSelect = "SingleSelect";
            public const string MultiSelect = "MultiSelect";
            public const string Range = "Range";
        };

        private static PromotedFacetValueSetting ConvertToPromotedFacetValueSetting(IPromotedFacetValueSetting promotedFacetValueSetting)
        {
            return new PromotedFacetValueSetting(promotedFacetValueSetting.Title)
            {
                SortWeight = (double)promotedFacetValueSetting.SortWeight,
            };
        }

    };
}
