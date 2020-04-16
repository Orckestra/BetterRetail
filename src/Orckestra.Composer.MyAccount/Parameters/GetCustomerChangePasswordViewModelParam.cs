using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class GetCustomerChangePasswordViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory) 
        /// Customer targeted by the change password results
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory) 
        /// The scope where to get the customer.
        /// </summary>
        public string Scope { get; set; }
    }
}
