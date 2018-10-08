using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to create a new User Account
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// (Mandatory)
        /// The Firstname for the User account
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The Lastname for the User account
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        /// <summary>
        /// (Mandatory)
        /// A unique Email address to bind to the new User account
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; set; }

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
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

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
        /// ReturnUrl to be used on client side to redirect after logout
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
