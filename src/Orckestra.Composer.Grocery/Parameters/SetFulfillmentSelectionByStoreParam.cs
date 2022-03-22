using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class SetFulfillmentSelectionByStoreParam
    {
        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Store Number
        /// </summary>
        public string StoreNumber { get; set; }

        /// <summary>
        /// The cart customer id associated with the reservation
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The user is logged in
        /// </summary>
        public bool IsAuthenticated { get; set; }
    }
}
