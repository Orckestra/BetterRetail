using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class GetEditAddressViewModelAsyncParam
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
        /// The id of address to edit.
        /// </summary>
        public Guid AddressId { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope where to get the customer.
        /// </summary>
        public string Scope { get; set; }
    }
}
