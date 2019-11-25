using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.Search.Providers.Facet
{
    public class RangeFacetProvider : BaseFacetProvider, IFacetProvider
    {
        public RangeFacetProvider(IFacetLocalizationProvider facetLocalizationProvider)
            : base(facetLocalizationProvider)
        {
        }

        /// <summary>
        ///     Gets the type of facet this provider is building.
        /// </summary>
        protected override FacetType FacetType
        {
            get { return FacetType.Range; }
        }

        protected override Facets.Facet BuildFacet(
            Overture.ServiceModel.Search.Facet facetResult, 
            FacetSetting setting,
            List<FacetValue> facetValues, 
            CultureInfo cultureInfo)
        {
            var facet = new Facets.Facet
            {
                Title = facetResult.Title,
                FieldName = facetResult.FieldName,
                Quantity = facetValues.Sum(value => value.Quantity),
                FacetType = FacetType,
                SortWeight = setting.SortWeight,
                StartValue = facetResult.StartValue,
                EndValue = facetResult.EndValue,
                GapSize = facetResult.GapSize,
                IsDisplayed = setting.IsDisplayed
            };

            //In order to always see selected facet values
            var selectedValueCount = facetValues.Count(x => x.IsSelected);
            var maxCollapsedValueCount = selectedValueCount > setting.MaxCollapsedValueCount
                ? selectedValueCount
                : setting.MaxCollapsedValueCount;

            facet.FacetValues = facetValues.Take(maxCollapsedValueCount).ToList();
            facet.OnDemandFacetValues = facetValues
                .Skip(maxCollapsedValueCount)
                .Take(setting.MaxExpendedValueCount - setting.MaxCollapsedValueCount)
                .ToList();

            return facet;
        }

        protected override bool HasAllFacetValuesSelected(Overture.ServiceModel.Search.Facet facetResult,
            IReadOnlyCollection<string> selectedFacetValues)
        {
            return false;
        }

        protected override IEnumerable<string> TransformFacetSelectedValue(
            Overture.ServiceModel.Search.Facet facetResult, string facetSelectedValue)
        {
            var selectedFacetValues = facetSelectedValue.Split(SearchConfiguration.FacetRangeValueSplitter);
            return selectedFacetValues;
        }
    }
}