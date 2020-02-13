using System.Collections.Generic;

namespace Orckestra.Composer.Cart
{
    /// <summary>
    /// The order history configuration.
    /// </summary>
    public static class OrderHistoryConfiguration
    {
        private static int _maxItemsPerPage = 10;
        private static int _maximumNumberOfPages = 5;
        private static List<string> _completedOrderStatuses = new List<string> { "Shipped", "Canceled", "Completed" };

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public static int MaxItemsPerPage 
        {
            get { return _maxItemsPerPage; }
            set { _maxItemsPerPage = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of pages.
        /// </summary>
        /// <value>
        /// The maximum number of pages.
        /// </value>
        public static int MaximumNumberOfPages
        {
            get { return _maximumNumberOfPages; }
            set { _maximumNumberOfPages = value; }
        }

        /// <summary>
        /// Gets or sets the completed order statuses.
        /// </summary>
        /// <value>
        /// The past order statuses.
        /// </value>
        public static List<string> CompletedOrderStatuses
        {
            get { return _completedOrderStatuses; }
            set { _completedOrderStatuses = value; }
        }
    }
}