using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class RecoverUserFromCookieDataParam
    {
        /// <summary>
        /// Culture info for retrieving localized data
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The cart customer id associated with the reservation
        /// </summary>
        public Guid CustomerId { get; set; }
    }
}
