using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.Search.Factory
{
    public interface IFacetFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="Facet"/> based on a <param name="facet"></param> object.
        /// </summary>
        Facet CreateFacet(Overture.ServiceModel.Search.Facet facet, IReadOnlyList<SearchFilter> selectedFacets,
            CultureInfo cultureInfo);
    }
}