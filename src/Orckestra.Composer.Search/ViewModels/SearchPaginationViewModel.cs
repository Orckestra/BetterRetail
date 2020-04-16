using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    /// <summary>
    /// Search pagination
    /// </summary>
    public sealed class SearchPaginationViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets properties for the [Previous] page button.
        /// </summary>
        /// <value>
        /// The previous page.
        /// </value>
        public SearchPageViewModel PreviousPage { get; set; }

        /// <summary>
        /// Gets or sets properties for the [Next] page button.
        /// </summary>
        /// <value>
        /// The next page.
        /// </value>
        public SearchPageViewModel NextPage { get; set; }

        /// <summary>
        /// Gets or sets properties for the [Current] page button.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public SearchPageViewModel CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets properties for the paged result set.
        /// </summary>
        /// <value>
        /// The pages.
        /// </value>
        public IEnumerable<SearchPageViewModel> Pages { get; set; }

        public int TotalNumberOfPages { get; set; }

    }
}