using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Service call params to change the password for a given Customer
    /// </summary>
    public class ChangePasswordParam
    {
        /// <summary>
        /// (Mandatory)
        /// The unique identifier for the customer to update
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The current password for the customer to update
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The new password to set
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope responsible for this request, to which the Customer must belong
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
