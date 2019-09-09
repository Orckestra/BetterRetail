using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetCustomerOrdersParam
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        /// <value>
        /// The culture information.
        /// </value>
        public CultureInfo CultureInfo { get; set; }
        public Guid WebsiteId { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the fulfillment location identifier.
        /// </summary>
        /// <value>
        /// The fulfillment location identifier.
        /// </value>
        public Guid? FulfillmentLocationId { get; set; }

        /// <summary>
        /// Gets or sets the order tense.
        /// </summary>
        /// <value>
        /// The order tense.
        /// </value>
        public OrderTense OrderTense { get; set; }
    }
}
