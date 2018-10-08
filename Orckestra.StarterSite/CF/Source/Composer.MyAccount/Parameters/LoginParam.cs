using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Service call params to authenticate a User
    /// </summary>
    public class LoginParam
    {
        /// <summary>
        /// (Mandatory)
        /// The unique login Name of the User to authenticate
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The login Password of the User to authenticate
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The culture for any displayed values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope for which the user needs to be authenticated
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Guest customer id used to merge carts.
        /// </summary>
        public Guid GuestCustomerId { get; set; }
    }
}