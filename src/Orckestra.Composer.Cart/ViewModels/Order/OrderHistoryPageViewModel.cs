using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderHistoryPageViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the display name for a paged result
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the paged result is the current paged result.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is the current page in the search pagination; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentPage { get; set; }
    }
}
