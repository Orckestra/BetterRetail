using System;
using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetLineItemsParam
    {
        /// <summary>
        /// Id of a scope where to find line items of a cart
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Unique identifier of a customer, who owns a cart
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The name associated to the requested cart
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// Culture info in which language data to be returned
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}