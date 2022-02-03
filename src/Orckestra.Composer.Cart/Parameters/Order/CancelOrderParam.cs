using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class CancelOrderParam
    {
        /// <summary>
        /// Gets or sets the order id
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// The culture information.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        public Guid CustomerId { get; set; }
    }
}
