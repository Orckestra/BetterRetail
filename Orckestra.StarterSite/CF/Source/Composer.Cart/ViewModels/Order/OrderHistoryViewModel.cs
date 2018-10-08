using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class OrderHistoryViewModel: BaseViewModel
    {
        /// <summary>
        /// Gets or sets the pagination.
        /// </summary>
        /// <value>
        /// The pagination.
        /// </value>
        public OrderHistoryPaginationViewModel Pagination { get; set; }

        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        /// <value>
        /// The orders.
        /// </value>
        public IList<LightOrderDetailViewModel> Orders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this view model is being fetched via AJAX.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading { get; set; }

        public OrderHistoryViewModel()
        {
            Orders = new List<LightOrderDetailViewModel>();
        }
    }
}
