using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.Search.Providers.Facet
{
    public abstract class BaseFacetProvider
    {
        protected IFacetLocalizationProvider FacetLocalizationProvider { get; private set; }

        /// <summary>
        /// Gets the type of facet this provider is building.
        /// </summary>
        protected abstract FacetType FacetType { get; }

        protected BaseFacetProvider(IFacetLocalizationProvider facetLocalizationProvider)
        {
            FacetLocalizationProvider = facetLocalizationProvider ?? throw new ArgumentNullException(nameof(facetLocalizationProvider));
        }


        /// <summary>
        /// Creates a new instance of a <see cref="Facet"/> from a <see cref="facetResult"/> object.
        /// </summary>
        /// <param name="facetResult">Facet to create the facet predicate from.</param>
        /// <param name="setting">Settings of the facet</param>
        /// <param name="selectedFacets">List of selected facet to determine if the values of the facet were selected or not</param>
        /// <param name="cultureInfo">Culture in which the display names will be returned in</param>
        public virtual Facets.Facet CreateFacet(Overture.ServiceModel.Search.Facet facetResult, FacetSetting setting, IReadOnlyList<SearchFilter> selectedFacets, CultureInfo cultureInfo)
        {
            if (facetResult == null) { throw new ArgumentNullException(nameof(facetResult)); }
            if (setting == null) { throw new ArgumentNullException(nameof(setting)); }

            if (!setting.FieldName.Equals(facetResult.FieldName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    string.Format("The specified setting is for the facet '{0}', whereas the facetResult is for the facet '{1}'", 
                    setting.FieldName, facetResult.FieldName), nameof(setting));
            }

            if (setting.FacetType != FacetType)
            {
                throw new ArgumentException(
                    string.Format("The facetResult is defined as '{0}' which does not match '{1}'", 
                    setting.FacetType, FacetType), nameof(setting));
            }

            var selectedFacetValues = GetSelectedFacetValues(facetResult, selectedFacets);

            // For field facets, return only those whose values were not all selected
            if (HasAllFacetValuesSelected(facetResult, selectedFacetValues)) { return null; }

            var facetValues = GetFacetValues(facetResult, setting, selectedFacetValues, cultureInfo);

            var facet = BuildFacet(facetResult, setting, facetValues, cultureInfo);

            return facet;
        }

        protected virtual Facets.Facet BuildFacet(Overture.ServiceModel.Search.Facet facetResult, FacetSetting setting, List<FacetValue> facetValues, CultureInfo cultureInfo)
        {
            var facet = new Facets.Facet
            {
                Title = facetResult.Title,
                FieldName = facetResult.FieldName,
                Quantity = facetValues.Count,
                FacetType = FacetType,
                SortWeight = setting.SortWeight,
                IsDisplayed = !facetValues.Any(value => value.IsPromoted) && setting.IsDisplayed
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

        protected virtual bool HasAllFacetValuesSelected(Overture.ServiceModel.Search.Facet facetResult, IReadOnlyCollection<string> selectedFacetValues)
        {
            var areAllFacetValuesSelected = facetResult.Values
                .All(
                    facetValue => selectedFacetValues
                        .Any(
                            selectedFacetValue =>
                                selectedFacetValue.Equals(facetValue.Value, StringComparison.OrdinalIgnoreCase)) &&
                                  facetResult.Count == selectedFacetValues.Count);

            return areAllFacetValuesSelected;
        }

        protected virtual List<string> GetSelectedFacetValues(Overture.ServiceModel.Search.Facet facetResult, IReadOnlyCollection<SearchFilter> selectedFacets)
        {
            //Get the criteria value for this facet
            var facetSelectedValue = selectedFacets
                .Where(selectedFacet => selectedFacet.Name == facetResult.FieldName)
                .Select(selectedFacet => selectedFacet.Value)
                .FirstOrDefault();

            var selectedFacetValues = facetSelectedValue == null
                ? new List<string>()
                : TransformFacetSelectedValue(facetResult, facetSelectedValue).ToList();

            return selectedFacetValues;
        }

        protected abstract IEnumerable<string> TransformFacetSelectedValue(Overture.ServiceModel.Search.Facet facetResult, string facetSelectedValue);


        protected virtual List<FacetValue> GetFacetValues(
            Overture.ServiceModel.Search.Facet facetResult, FacetSetting setting,
            IReadOnlyCollection<string> selectedFacetValues, CultureInfo cultureInfo)
        {
            var promotedValues = setting.PromotedValues;

            /*Expected to be already sorted*/
            var facetValues = facetResult.Values
                .Select(resultFacetValue =>
                {
                    var facetValue = new FacetValue
                    {
                        Title = FacetLocalizationProvider.GetFormattedFacetValueTitle(facetResult.FieldName, resultFacetValue.Value, cultureInfo),
                        Value = resultFacetValue.Value,
                        Quantity = resultFacetValue.Count,
                        IsSelected = selectedFacetValues.Contains(resultFacetValue.Value),
                        MinimumValue = resultFacetValue.MinimumValue,
                        MaximumValue = resultFacetValue.MaximumValue
                    };

                    var promotedValueSetting = promotedValues.FirstOrDefault(
                        value => value.Title.Equals(resultFacetValue.Value, StringComparison.OrdinalIgnoreCase));

                    if (promotedValueSetting != null)
                    {
                        facetValue.IsPromoted = true;
                        facetValue.PromotionSortWeight = promotedValueSetting.SortWeight;
                    }

                    return facetValue;
                })
                .ToList();

            facetValues = facetValues.OrderByDescending(x => x.IsSelected).ToList();
            return facetValues;
        }
    }
}