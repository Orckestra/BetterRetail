using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class GetCreateAddressViewModelAsyncParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The id of the customer who creates a new address.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The country code of the customer.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope where to get the customer.
        /// </summary>
        public string Scope { get; set; }
    }
}
