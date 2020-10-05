using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class RecoverCookieDataParam
    {
        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The cart name associated with the reservation
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// The cart customer id associated with the reservation
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the customer's preferred store.
        /// </summary>
        public Guid PreferredStoreId { get; set; }
    }
}
