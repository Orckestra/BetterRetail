using Orckestra.Overture.ServiceModel.Search;
using SearchFilter = Orckestra.Composer.Parameters.SearchFilter;

namespace Orckestra.Composer.Search.Factory
{
    public interface IFacetPredicateFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="FacetPredicate"/> based on a <param name="filter"></param> object.
        /// </summary>
        FacetPredicate CreateFacetPredicate(SearchFilter filter);
    }
}