using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public abstract class GetOrderParam
    {
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the Number of the order to retrieve.
        /// </summary>
        /// <value>
        /// The Order Number.
        /// </value>
        public string OrderNumber { get; set; }

        /// <summary>
        /// The Country iso code of the order.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The base Url.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The culture information.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
