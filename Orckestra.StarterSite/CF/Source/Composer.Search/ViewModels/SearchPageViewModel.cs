using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Search.ViewModels
{
    /// <summary>
    /// Contains information in regards to a search paged result.
    /// </summary>
    public sealed class SearchPageViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the display name for a search paged result
        /// </summary>
        public string DisplayName { get; set; }

        public string UrlPath { get; set; }

        /// <summary>
        /// Gets or sets the URL for a search paged result.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search paged result is the current paged search result.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is the current page in the search pagination; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentPage { get; set; }

    }
}