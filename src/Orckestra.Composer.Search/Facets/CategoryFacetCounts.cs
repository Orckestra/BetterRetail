using System.Collections.Generic;

namespace Orckestra.Composer.Search.Facets
{
    /// <summary>
    /// Independent product counts for each category and total count for the search results wihtout category facets apllied
    /// </summary>
    public sealed class CategoryFacetCounts
    {
        /// <summary>
        /// Gets or sets the facets.
        /// </summary>
        /// <value>
        /// The facets.
        /// </value>
        public IList<Facet> Facets { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total count for the search results without category facets apllied.
        /// </value>
        public long TotalCount { get; set; }
    }
}
