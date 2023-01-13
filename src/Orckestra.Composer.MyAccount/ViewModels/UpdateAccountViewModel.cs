using System.Collections.Generic;
using Orckestra.Composer.ViewModels;
using ServiceStack;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Create Account Form
    /// </summary>
    public sealed class UpdateAccountViewModel : BaseViewModel
    {
        /// <summary>
        ///  The FirstName for the User account
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///  The LastName for the User account
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Creation Status result (Success, DuplicateEmail, UserRejected, Failed, ...)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Url to the change password page
        /// </summary>
        public string ChangePasswordUrl { get; set; }

        /// <summary>
        /// Url to the address list page
        /// </summary>
        public string AddressListUrl { get; set; }

        /// <summary>
        ///  The primary and unique email for this User account
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The unique name used for the User account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///  Customer preferred language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///  All the languages available for the customer preferred language.
        /// </summary>
        public Dictionary<string, string> Languages { get; set; }

        /// <summary>
        /// The PhoneNumber for the User account
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The PhoneNumber RegEx validation rule
        /// </summary>
        public string PhoneNumberRegEx { get; set; }

        /// <summary>
        /// Property Bag
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; set; }

        public UpdateAccountViewModel()
        {
            Languages = new Dictionary<string, string>();
            PropertyBag = new Dictionary<string, object>();
        }
    }
}
