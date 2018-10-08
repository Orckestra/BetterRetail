using System;
using System.Collections.Generic;
using Orckestra.Composer.Utils;
using Orckestra.Overture.ServiceModel.Search;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Providers.FacetPredicate
{
    public class SingleSelectFacetPredicateProvider : IFacetPredicateProvider
    {
        /// <summary>
        ///     Creates a new instance of a <see cref="FacetPredicate" /> from a <see cref="SearchFilter" /> object.
        /// </summary>
        /// <param name="filter">Filter to create the facet predicate from.</param>
        public Overture.ServiceModel.Search.FacetPredicate CreateFacetPredicate(SearchFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            if (string.IsNullOrWhiteSpace(filter.Name))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Name"), "filter");
            }

            if (string.IsNullOrWhiteSpace(filter.Value))
            {
                return null;
            }

            var facetPredicate = new Overture.ServiceModel.Search.FacetPredicate
            {
                OperatorType = FacetValuesOperator.Or,
                ExcludeFilterForFacetsCount = true,
                FieldName = filter.Name,
                FacetType = FacetType.Field,
                Values = new List<string> {filter.Value}
            };

            return facetPredicate;
        }
    }
}