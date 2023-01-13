using System;
using System.Collections.Generic;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class UpdateAccountParam
    {
        /// <summary>
        /// (Mandatory)
        /// The id of the existing customer update
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory)
        /// A unique Email address to bind to the User account
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A username to bind to the User account
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The Firstname for the User account
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The Lastname for the User account
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope in which the User will be updated
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The preferred communication Culture iso code for the User
        /// </summary>
        public string PreferredLanguage { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The culture for any displayed values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Optional)
        /// The PhoneNumber for the User account
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// (Optional)
        /// Property Bag
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; set; }

    }
}
