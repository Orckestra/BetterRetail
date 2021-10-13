using Orckestra.Composer.Search.Facets;
using System.Collections.Generic;

namespace Orckestra.Composer.Search.Facets
{
    /// <summary>
    /// Independent product counts for each category and total count for the search results wihtout categories apllied
    /// </summary>
    public sealed class CategoryFacetCounts
    {
        public CategoryFacetCounts()
        {
        }

        /// <summary>
        /// Gets or sets the facet groups.
        /// </summary>
        /// <value>
        /// The facet groups.
        /// </value>
        public IList<Facet> Facets { get; set; }

        /// <summary>
        /// Gets or sets the total count for the search results without categories applied
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public long TotalCount { get; set; }

    }
}
