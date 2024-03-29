﻿using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class CreateCartOrderDraftParam
    {
        /// <summary>
        /// Gets or sets the order id
        /// </summary>
        public Guid OrderId { get; set; }

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
