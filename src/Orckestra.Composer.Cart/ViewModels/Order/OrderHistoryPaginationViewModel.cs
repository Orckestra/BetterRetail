using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    /// <summary>
    /// Search pagination
    /// </summary>
    public sealed class OrderHistoryPaginationViewModel: BaseViewModel
    {
        /// <summary>
        /// Gets or sets properties for the [Previous] page button.
        /// </summary>
        /// <value>
        /// The previous page.
        /// </value>
        public OrderHistoryPageViewModel PreviousPage { get; set; }

        /// <summary>
        /// Gets or sets properties for the [Next] page button.
        /// </summary>
        /// <value>
        /// The next page.
        /// </value>
        public OrderHistoryPageViewModel NextPage { get; set; }

        /// <summary>
        /// Gets or sets properties for the paged result set.
        /// </summary>
        /// <value>
        /// The pages.
        /// </value>
        public IEnumerable<OrderHistoryPageViewModel> Pages { get; set; }
    }
}