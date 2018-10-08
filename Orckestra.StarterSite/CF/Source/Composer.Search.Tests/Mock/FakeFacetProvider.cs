using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers.Facet;

namespace Orckestra.Composer.Search.Tests.Mock
{
    public class FakeFacetProvider : IFacetProvider
    {
        /// <summary>
        ///     Creates a new instance of a <see cref="Facets.Facet" /> from a <see cref="facetResult" /> object.
        /// </summary>
        /// <param name="facetResult">Facet to create the facet predicate from.</param>
        /// <param name="setting">Settings of the facet</param>
        /// <param name="selectedFacets">List of selected facet to determine if the values of the facet were selected or not</param>
        /// <param name="cultureInfo">Culture in which the display names will be returned in</param>
        public Facet CreateFacet(Overture.ServiceModel.Search.Facet facetResult, FacetSetting setting,
            IReadOnlyList<SearchFilter> selectedFacets, CultureInfo cultureInfo)
        {
            return new Facet();
        }
    }
}