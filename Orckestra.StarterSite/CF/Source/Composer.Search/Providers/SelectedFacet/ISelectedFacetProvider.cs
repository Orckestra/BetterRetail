
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Search.Providers.SelectedFacet
{
    public interface ISelectedFacetProvider
    {
        /// <summary>
        /// Creates a new list of a <see cref="Facets.SelectedFacet"/> from a <see cref="filter"/> object.
        /// </summary>
        /// <param name="filter">Facet to create the facet predicate from.</param>
        /// <param name="setting">Settings of the facet</param>
        /// <param name="cultureInfo">Culture in which the display names will be returned in</param>
        IEnumerable<Facets.SelectedFacet> CreateSelectedFacetList(SearchFilter filter, FacetSetting setting, CultureInfo cultureInfo); 
    }
}