using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Repository call param to retreive all addresses for a Customer
    /// </summary>
    public class GetAddressesForCustomerParam 
    {
        /// <summary>
        /// (Mandatory)
        /// The unique Id of the customer to look for
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope to which the Addresses must belong
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The Culture for any displayable values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
