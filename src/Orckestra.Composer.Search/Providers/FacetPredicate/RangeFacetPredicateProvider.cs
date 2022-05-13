using System;
using Orckestra.Overture.ServiceModel.Search;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Providers.FacetPredicate
{
    public class RangeFacetPredicateProvider : IFacetPredicateProvider
    {
        /// <summary>
        /// Creates a new instance of a <see cref="FacetPredicate"/> from a <see cref="SearchFilter"/> object.
        /// </summary>
        /// <param name="filter">Filter to create the facet predicate from.</param>
        public Overture.ServiceModel.Search.FacetPredicate CreateFacetPredicate(SearchFilter filter)
        {
            if (filter == null) { throw new ArgumentNullException(nameof(filter)); }
            if (string.IsNullOrWhiteSpace(filter.Name)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(filter.Name)), nameof(filter)); }

            if (string.IsNullOrWhiteSpace(filter.Value)) { return null; }

            var valueRanges = filter.Value.Split(SearchConfiguration.FacetRangeValueSplitter);

            // TODO: A SEG request (#2259) was sent to Overture to fix this as a range facet predicate always needs the minimum value to be set.
            // In the meantime, we have to pass "*" if the minimum value is not set in the search filter
            var minimumValue = string.IsNullOrWhiteSpace(valueRanges[0]) ? "*" : valueRanges[0];

            // TODO: A SEG request (#2259) was sent to Overture to fix this as a range facet predicate always needs the maximum value to be set.
            // In the meantime, we have to pass "*" if the maximum value is not set in the search filter
            var maximumValue = valueRanges.Length > 1 && !string.IsNullOrWhiteSpace(valueRanges[1]) ? valueRanges[1] : "*";

            var facetPredicate = new Overture.ServiceModel.Search.FacetPredicate
            {
                ExcludeFilterForFacetsCount = true,
                FieldName = filter.Name,
                FacetType = FacetType.Range,
                MinimumValue = minimumValue,
                MaximumValue = maximumValue
            };

            return facetPredicate;
        }
    }
}