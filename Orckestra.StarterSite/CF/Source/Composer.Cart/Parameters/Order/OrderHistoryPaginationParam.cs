using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class OrderHistoryPaginationParam
    {
        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the current page of the paged set of orders.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of orders.
        /// </summary>
        /// <value>
        /// The total orders.
        /// </value>
        public int TotalNumberOfOrders { get; set; }
    }
}
