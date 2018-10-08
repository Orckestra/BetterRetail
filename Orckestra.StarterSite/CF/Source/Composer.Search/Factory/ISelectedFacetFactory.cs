using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Facets;

namespace Orckestra.Composer.Search.Factory
{
    public interface ISelectedFacetFactory
    {
        /// <summary>
        /// Creates a new list of <see cref="SelectedFacet"/> based on a <param name="filter"></param> object.
        /// </summary>
        IEnumerable<SelectedFacet> CreateSelectedFacet(SearchFilter filter, CultureInfo cultureInfo);
    }
}