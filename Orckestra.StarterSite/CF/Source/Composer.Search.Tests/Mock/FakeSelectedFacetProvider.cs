using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Providers.SelectedFacet;

namespace Orckestra.Composer.Search.Tests.Mock
{
    public class FakeSelectedFacetProvider : ISelectedFacetProvider
    {
        /// <summary>
        ///     Creates a new list of a <see cref="Facets.SelectedFacet" /> from a <see cref="filter" /> object.
        /// </summary>
        /// <param name="filter">Facet to create the facet predicate from.</param>
        /// <param name="setting">Settings of the facet</param>
        /// <param name="cultureInfo">Culture in which the display names will be returned in</param>
        public IEnumerable<SelectedFacet> CreateSelectedFacetList(SearchFilter filter, FacetSetting setting,
            CultureInfo cultureInfo)
        {
            return new List<SelectedFacet> {new SelectedFacet()};
        }
    }
}