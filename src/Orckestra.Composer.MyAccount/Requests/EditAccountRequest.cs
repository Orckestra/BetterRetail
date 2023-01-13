using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.MyAccount.Requests
{
    public class EditAccountRequest
    {
        /// <summary>
        /// (Mandatory)
        /// A unique Email address to bind to the User account
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// A Username to bind to the User account
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The Firstname for the User account
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The Lastname for the User account
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// (Optional)
        /// The PhoneNumber for the User account
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The preferred communication Culture iso code for the User
        /// </summary>
        [Required]
        public string PreferredLanguage { get; set; }

        /// <summary>
        /// (Optional)
        /// Property Bag
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; set; }
    }
}
