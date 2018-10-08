using Orckestra.Overture.ServiceModel.Search;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Providers.FacetPredicate
{
    public interface IFacetPredicateProvider
    {
        /// <summary>
        /// Creates a new instance of a <see cref="FacetPredicate"/> from a <see cref="SearchFilter"/> object.
        /// </summary>
        /// <param name="filter">Filter to create the facet predicate from.</param>
        Overture.ServiceModel.Search.FacetPredicate CreateFacetPredicate(SearchFilter filter);
    }
}