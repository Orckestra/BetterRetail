using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Providers.FacetPredicate;
using Orckestra.Overture.ServiceModel.Search;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Tests.Mock
{
    public class FakeFacetPredicateProvider:IFacetPredicateProvider
    {
        /// <summary>
        /// Creates a new instance of a <see cref="FacetPredicate"/> from a <see cref="SearchFilter"/> object.
        /// </summary>
        /// <param name="filter">Filter to create the facet predicate from.</param>
        public FacetPredicate CreateFacetPredicate(SearchFilter filter)
        {
            return new FacetPredicate();
        }
    }
}