using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    /// <summary>
    /// Search criteria used to return search results.
    /// </summary>
	public class SearchCriteria
    {
        public SearchCriteria()
        {
            InventoryLocationIds = new List<string>();
            SelectedFacets = new List<SearchFilter>();
        }

        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets whether or not to include facets in search results.
        /// </summary>
        /// <value>
        ///   <c>true</c> if facets are to be included; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeFacets { get; set; }

        /// <summary>
        /// Gets or sets the keywords to search for.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or sets the number of search results to display per paged search result.
        /// </summary>
        public int NumberOfItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the sales scope to search in.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the criteria to sort by.
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        public string SortDirection { get; set; }

        /// <summary>
        /// Gets or sets the starting index of the search result.
        /// </summary>
        /// <value>
        /// The index of the starting.
        /// </value>
        public int StartingIndex { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the base search URL.
        /// </summary>
        /// <value>
        /// The search URL.
        /// </value>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public List<SearchFilter> SelectedFacets { get; private set; }

        public List<string> InventoryLocationIds { get; set; }

        public bool AutoCorrect { get; set; }

	    public SearchCriteria Clone()
	    {
		    var clone = (SearchCriteria)MemberwiseClone();
		    clone.SelectedFacets = new List<SearchFilter>();
            clone.InventoryLocationIds = new List<string>();

		    foreach (var selectedFacet in SelectedFacets)
		    {
			    clone.SelectedFacets.Add(selectedFacet.Clone());
		    }

	        if (InventoryLocationIds != null)
	        {
                clone.InventoryLocationIds.AddRange(InventoryLocationIds);
	        }

		    return clone;
	    }
    }
}
