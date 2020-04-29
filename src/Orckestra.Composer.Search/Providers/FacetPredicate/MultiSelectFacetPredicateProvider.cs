using System;
using System.Linq;
using Orckestra.Overture.ServiceModel.Search;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Providers.FacetPredicate
{
    public class MultiSelectFacetPredicateProvider : IFacetPredicateProvider
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

            var values = filter.Value.Split(SearchConfiguration.MultiFacetValueSplitter).ToList();

            var facetPredicate = new Overture.ServiceModel.Search.FacetPredicate
            {
                OperatorType = FacetValuesOperator.Or,
                ExcludeFilterForFacetsCount = true,
                FieldName = filter.Name,
                FacetType = FacetType.Field,
                Values = values
            };

            return facetPredicate;
        }
    }
}