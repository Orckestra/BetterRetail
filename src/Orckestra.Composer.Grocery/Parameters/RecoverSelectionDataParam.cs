using System;
using System.Globalization;

namespace Orckestra.Composer.Grocery.Parameters
{
    public class RecoverSelectionDataParam
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

        public bool IsAuthenticated { get; set; }
    }
}
