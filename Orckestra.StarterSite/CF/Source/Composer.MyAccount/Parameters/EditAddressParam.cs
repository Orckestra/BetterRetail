using System;
using System.Globalization;
using Orckestra.Composer.MyAccount.Requests;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Service call params to Create or Update the information for a Customer's address
    /// </summary>
    public class EditAddressParam
    {
        /// <summary>
        /// (Mandatory)
        /// The unique identifier for the customer to who the address must belong
        /// </summary>
        public Guid CustomerId { get; set; }

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

        /// <summary>
        /// (Mandatory) 
        ///  The edited address
        /// </summary>
        public EditAddressRequest EditAddress { get; set; }

        /// <summary>
        /// (Mandatory) 
        ///  The Id of the address to edit
        /// </summary>
        public Guid AddressId { get; set; }
    }
}
