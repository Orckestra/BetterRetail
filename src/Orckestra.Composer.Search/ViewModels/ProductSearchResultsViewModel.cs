using System.Collections.Generic;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    /// <summary>
    /// The view model for search results.
    /// </summary>
    public sealed class ProductSearchResultsViewModel : BaseViewModel
    {
        public ProductSearchResultsViewModel()
        {
            SearchResults = new List<ProductSearchViewModel>();
            Facets = new List<Facet>();
            PromotedFacetValues = new List<PromotedFacetValue>();
            AvailableSortBys = new List<SortBy>();
            Suggestions = new List<Suggestion>();
        }

        /// <summary>
        /// Gets or sets the search results.
        /// </summary>
        /// <value>
        /// The search results.
        /// </value>
        public IList<ProductSearchViewModel> SearchResults { get; set; }

        /// <summary>
        /// Get or set independent product counts for category facets
        /// </summary>
        public CategoryFacetCounts CategoryFacetCounts { get; set; }

        /// <summary>
        /// Gets or sets the facet groups.
        /// </summary>
        /// <value>
        /// The facet groups.
        /// </value>
        //TODO This property property should be renamed "Facets" to preserve the meaning of Facets
        public IList<Facet> Facets { get; set; }
        
        /// <summary>
        /// Gets or sets the promoted facet values
        /// </summary>
        public IList<PromotedFacetValue> PromotedFacetValues { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>
        /// The keywords.
        /// </value>
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or sets the total count for the search results.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public long TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the selected sort by for the current search results.
        /// </summary>
        /// <value>
        /// The selected sort by.
        /// </value>
        public SelectedSortBy SelectedSortBy { get; set; }

        /// <summary>
        /// Gets or sets the available "sort by" list available to search.
        /// </summary>
        /// <value>
        /// The available sort bys.
        /// </value>
        public IList<SortBy> AvailableSortBys { get; set; }

        /// <summary>
        /// Gets or sets the pagination for search results.
        /// </summary>
        /// <value>
        /// The pager.
        /// </value>
        public SearchPaginationViewModel Pagination { get; set; }

        /// <summary>
        /// The auto-corrected search terms used to return values
        /// </summary>
        public string CorrectedSearchTerms { get; set; }

        /// <summary>
        /// A list of Suggestion that could return more meaningful results for the search term entered
        /// </summary>
        public IList<Suggestion> Suggestions { get; set; }

        public string BaseUrl { get; set; }
    }
}
