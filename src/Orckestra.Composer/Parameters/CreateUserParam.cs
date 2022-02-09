using System;
using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    /// <summary>
    /// Service call params to create a new User Account
    /// </summary>
    public class CreateUserParam
    {
        /// <summary>
        /// (Optional)
        /// The unique login Name
        /// If omitted, the Email will be used
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The login password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// (Optional)
        /// A unique Identifier to bind to the new User account
        /// It is best practice to omit
        /// If omitted, a new Id will be generated
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// (Mandatory)
        /// A unique Email address to bind to the new User account
        /// </summary>
        public string Email { get; set; }

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
        /// Phone number of a user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The scope in which the User will be created
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The prefered communication Culture for the new User
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Optional)
        /// A secret question to ask on forgot password
        /// </summary>
        public string PasswordQuestion { get; set; }

        /// <summary>
        /// (Optional)
        /// The secret anwser matching the password question
        /// </summary>
        public string PasswordAnswer { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Guest customer id used to merge carts.
        /// </summary>
        public Guid GuestCustomerId { get; set; }

        public CreateUserParam Clone()
        {
            var param = (CreateUserParam)MemberwiseClone();
            return param;
        }
    }
}
